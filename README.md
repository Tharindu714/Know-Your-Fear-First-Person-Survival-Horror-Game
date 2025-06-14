![Know Your Fear Main Image](https://github.com/user-attachments/assets/2c4d4908-48c6-4486-a269-ac3a897fe32b)

# 🎮 Know Your Fear: First Person Horror Survival Game

**Version 1.0**

---

## 📦 Installation

1. **Download** the ZIP or installer from the GitHub Releases page.
2. **Extract** the folder or run the installer. Ensure the `Know Your Fear.exe` and `Know Your Fear_Data` folder remain together.
3. **Run** `Know Your Fear.exe` (Windows) or the `.app` bundle on macOS.

Requires Windows 10+ (DirectX 11) or macOS 10.13+.

---

## 🎯 Quick Start

* **Move**: W / A / S / D
* **Jump**: Spacebar
* **Interact**: E
* **Open Box**: O
* **Pick Up Toys**: G
* **Toggle Candle**: L
* **Recording Mode**: R
* **Night Vision**: N
* **Quick Spin**: C (only during panic)

---

## 🔍 Features

* **Health & Fear Meters**: Stay alive and sane—rest at candles, but fear summons ghosts.
* **JumpScare Meter**: Seven scares → forced collapse and game over.
* **Three Ghost Types**: CandleGhost (snuffs candles), Crawler (emerges below), Stalker (follows you).
* **10‑Minute Recording Mode**: Press R to start a timed survival challenge with optional Night Vision.
* **Achievements**: 12 letters, 11 toys, 5 scares, 5 spins, first CandleGhost—popups with icon + bell sound.

---

## ⚙️ Technical Details

* **Unity Version**: 2022.3 LTS (Built‑in RP)
* **Language**: C# (Mono)
* **Key Managers**:

  * `UIManager` (UI updates)
  * `JumpScareManager` (fear/scare logic)
  * `AchievementManager` (trophy popups)
  * `RecorderUIController` (recording overlay)
  * `CandleGhostManager`, `FollowingGhostManager`, `CrawlerManager` (AI spawners)
  * `Interaction` script (raycast-based input)

---

## 🧪 Testing & QA

* All mechanics tested via Play Mode and standalone builds.
* Performance profiled during high ghost counts.
* Known issue: Low‑res (<1280×720) UI clipping.

---

## 🎥 Media & Art

* **Models**: Optimized manor props & ghost rigs.
* **Audio**: Ambient tension, jump cues, BGM for win/lose.
* **UI**: TextMeshPro, custom icons, CanvasGroup fade animations.

---

## 📄 License & Credits

© 2025 \Insane Games By Tharindu Chanaka. All rights reserved.
Built with Unity.

---

*Press any key to start…*


