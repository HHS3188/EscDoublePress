# EscDoublePress

English | [中文](#中文) | [한국어](#한국어)

EscDoublePress is a Unity Mod Manager mod for **A Dance of Fire and Ice**. It prevents accidental Esc presses by requiring a second Esc press within a configurable time window.

## Features

- Requires double-pressing Esc before pausing official gameplay.
- Also works in the custom level editor playtest mode before returning to edit mode.
- Does not patch real gameplay failures or UI pause buttons.
- Configurable double-press window from 100 ms to 2000 ms.
- Built-in UI language selector: CN / EN / KR.

## Compatibility

Tested on A Dance of Fire and Ice release `r136` with Unity `2022.3.62f2`.

Compatibility with other game versions is not guaranteed. If you run into a problem, please open an [Issue](https://github.com/HHS3188/EscDoublePress/issues).

## Installation

1. Install Unity Mod Manager for A Dance of Fire and Ice.
2. Download `EscDoublePress-1.0.0.zip` from the release page.
3. Import the zip with Unity Mod Manager.
4. Start the game and enable `Esc Double Press Exit`.

## Build

This repository stores the mod source and a local build script. The script expects to be run from a copied mod folder inside an installed ADOFAI game directory, because it references the game's managed assemblies.

```powershell
.\Build.ps1
```

## 中文

EscDoublePress 是一个用于 **冰与火之舞** 的 Unity Mod Manager mod。它要求在指定时间窗口内第二次按下 Esc，避免误触 Esc 导致暂停或退出编辑器播放测试。

### 功能

- 官方关卡中需要双击 Esc 才会暂停。
- 自定义编辑器播放测试中需要双击 Esc 才会返回编辑模式。
- 不修改真实失败判定，也不影响 UI 暂停按钮。
- 双击判定窗口可设置为 100 ms 到 2000 ms。
- 设置面板内置语言切换：CN / EN / KR。

### 兼容性

目前已在冰与火之舞内部版本 `r136`、Unity `2022.3.62f2` 下测试通过。

不保证兼容其他游戏版本。如果遇到问题，可以开 [Issue](https://github.com/HHS3188/EscDoublePress/issues)。

### 安装

1. 为冰与火之舞安装 Unity Mod Manager。
2. 从 Release 页面下载 `EscDoublePress-1.0.0.zip`。
3. 用 Unity Mod Manager 导入该 zip。
4. 启动游戏并启用 `Esc Double Press Exit`。

## 한국어

EscDoublePress는 **A Dance of Fire and Ice**용 Unity Mod Manager 모드입니다. Esc 키를 실수로 눌렀을 때 바로 일시정지되거나 에디터 테스트 플레이에서 편집 모드로 돌아가는 것을 막기 위해, 설정된 시간 안에 Esc를 한 번 더 눌러야 동작합니다.

### 기능

- 공식 플레이에서 Esc를 두 번 눌러야 일시정지됩니다.
- 커스텀 레벨 에디터 테스트 플레이에서도 Esc를 두 번 눌러야 편집 모드로 돌아갑니다.
- 실제 실패 판정이나 UI 일시정지 버튼은 수정하지 않습니다.
- 두 번 누르기 판정 시간은 100 ms부터 2000 ms까지 설정할 수 있습니다.
- 설정 UI 언어 선택: CN / EN / KR.

### 호환성

현재 A Dance of Fire and Ice 내부 릴리스 `r136`, Unity `2022.3.62f2`에서 테스트되었습니다.

다른 게임 버전과의 호환성은 보장하지 않습니다. 문제가 있으면 [Issue](https://github.com/HHS3188/EscDoublePress/issues)를 열어 주세요.

### 설치

1. A Dance of Fire and Ice에 Unity Mod Manager를 설치합니다.
2. Release 페이지에서 `EscDoublePress-1.0.0.zip`을 다운로드합니다.
3. Unity Mod Manager로 zip 파일을 가져옵니다.
4. 게임을 시작하고 `Esc Double Press Exit`를 활성화합니다.
