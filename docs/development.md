# 构建、测试与发布

## 前置

本机需安装 .NET SDK（目标框架 `net10.0`）。依赖通过 NuGet 拉取（`SPTarkov.Server.Core`、`SPTarkov.DI`、`SPTarkov.Common`，版本对齐服务端 4.1.x），**无需本机 SPT 程序集**。

## 构建

```shellscript
dotnet build -c Release
```

产物为 `bin/Release/WeaponCrossModding.dll`。

## 测试

单元测试位于 `tests/WeaponCrossModding.Tests/`，用最小物品 / 槽位夹具直接驱动插件的槽位注入，覆盖各白名单追加、互斥关系与幂等：

```shellscript
dotnet test tests/WeaponCrossModding.Tests/WeaponCrossModding.Tests.csproj
```

## CI 自动发布

`.github/workflows/build.yml` 在推送 `v*` 标签（或手动触发）时自动构建并打包：

1. `dotnet restore` 与 `dotnet build -c Release`。
2. 运行 `dotnet test tests/WeaponCrossModding.Tests/WeaponCrossModding.Tests.csproj -c Release`。
3. 校验 `bin/Release/WeaponCrossModding.dll` 存在。
4. 从 csproj 读取 `<Version>`，按游戏根目录结构打包为 `WeaponCrossModding-v{版本}.zip`。
5. 上传 zip 为 workflow artifact；标签触发时创建 GitHub Release 并附带该 zip。

`Version`（csproj）应与标签保持一致（如标签 `v0.1.0` 对应 `<Version>0.1.0</Version>`）。

## 本机部署

将 `WeaponCrossModding.dll` 放入服务端 `user/mods/WeaponCrossModding/` 目录，然后重启服务端：

```shellscript
systemctl --user restart pj568-spt-server
```

也可以直接解压 Release 中的 `WeaponCrossModding-v{版本}.zip` 到对应目录。
部署后按[实机验证](verification.md)检查启动日志。

===============================================================

# Build, Test and Release

## Prerequisites

A .NET SDK is required (target framework `net10.0`). Dependencies are pulled from NuGet (`SPTarkov.Server.Core`, `SPTarkov.DI`, `SPTarkov.Common`, version-aligned with server 4.1.x), so **no local SPT assemblies are needed**.

## Build

```shellscript
dotnet build -c Release
```

The output is `bin/Release/WeaponCrossModding.dll`.

## Test

The unit tests live in `tests/WeaponCrossModding.Tests/`. They drive the plugin's slot injection directly with minimal item / slot fixtures, covering every whitelist addition, the conflict, and idempotency:

```shellscript
dotnet test tests/WeaponCrossModding.Tests/WeaponCrossModding.Tests.csproj
```

## CI Auto-Release

`.github/workflows/build.yml` builds and packages automatically when a `v*` tag is pushed (or on manual dispatch):

1. `dotnet restore` and `dotnet build -c Release`.
2. Run `dotnet test tests/WeaponCrossModding.Tests/WeaponCrossModding.Tests.csproj -c Release`.
3. Verify that `bin/Release/WeaponCrossModding.dll` exists.
4. Read `<Version>` from the csproj and package it as `WeaponCrossModding-v{version}.zip` using the game-root layout.
5. Upload the zip as a workflow artifact; on a tag push, create a GitHub Release with that zip attached.

`Version` (csproj) should stay in sync with the tag (e.g. tag `v0.1.0` ↔ `<Version>0.1.0</Version>`).

## Local Deployment

Place `WeaponCrossModding.dll` into the server's `user/mods/WeaponCrossModding/` directory, then restart the server:

```shellscript
systemctl --user restart pj568-spt-server
```

Alternatively, extract `WeaponCrossModding-v{version}.zip` from a Release into the corresponding directory.
After deploying, check the startup log as described in [In-game verification](verification.md).
