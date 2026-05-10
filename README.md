# ARDefense — AR Tower Defense Game

> A mobile tower defense game where you place turrets on real-world surfaces detected via AR and defend against waves of enemies across three historical eras.

---

## Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Tech Stack](#tech-stack)
- [Game Structure](#game-structure)
- [Architecture](#architecture)
- [Getting Started](#getting-started)

---

## Overview

**ARDefense** is an augmented reality tower defense game built with Unity and AR Foundation. The game detects a real-world horizontal surface (floor, desk, etc.) using the device camera, then generates a tile-grid game board on top of it. Players spend gold to place and upgrade turrets, surviving 9 waves of enemies (3 phases × 3 rounds) that grow progressively stronger as the game advances through the Middle Age, Modern Age, and Future Age eras.

---

## Features

### AR Board Generation
- Real-world horizontal plane detection via **ARPlaneManager**
- Procedural **tile grid (0.1 m/tile)** generation on detected surfaces
- Camera-aligned board orientation with rotational fit validation
- AR session coordinate reset on game restart — no scene reload required

### Wave & Phase System
| Phase | Waves | Era | Monster Sequence |
|-------|-------|-----|-----------------|
| Phase 1 | 1 – 3 | Middle Age | Normal → Elite → Boss |
| Phase 2 | 4 – 6 | Modern Age | Normal → Elite → Boss |
| Phase 3 | 7 – 9 | Future Age | Normal → Elite → Boss |

### Turret System (3 Types)
| Turret | Base HP | Base Damage | Attack Speed | Cost |
|--------|---------|-------------|--------------|------|
| Middle Age Turret | 10 | 5 | 2 / sec | 10 Gold |
| Modern Age Turret | 50 | 15 | 5 / sec | 25 Gold |
| Future Age Turret | 200 | 30 | 8 / sec | 50 Gold |

- All turrets are upgradeable (+10 HP, +10 Damage per upgrade, costs 20 Gold)
- Placement silhouette preview before confirming position

### Monster Scaling
| Monster | Base HP | Base Damage | Per Wave | Kill Reward |
|---------|---------|-------------|----------|-------------|
| Normal | 10 × 1.5^phase | 5 × 1.25^phase | 20 | 2 + 2×phase |
| Elite | 70 × 1.5^phase | 20 × 1.25^phase | 4 | 8 + 4×phase |
| Boss | 120 × 1.8^phase | 50 × 2.0^phase | 1 | 20 + 5×phase |

### Economy System
- Starting gold: **30**
- Gold is earned by defeating monsters and spent on turret placement and upgrades
- `GoldManager` singleton manages the balance and broadcasts changes via Unity Events

---

## Tech Stack

| Item | Details |
|------|---------|
| Engine | Unity 6000.3.10f |
| AR Framework | AR Foundation (ARPlaneManager, ARSession) |
| Language | C# |
| Target Platforms | Android (ARCore) / iOS (ARKit) |
| Input System | Enhanced Touch API (Mouse fallback in Editor) |

---

## Game Structure

```
GameFlowState
├── GAME_START            → AR plane detection & board placement
├── BEFORE_GATE_OPEN      → Preparation phase (place turrets)
├── NORMAL_MONSTER_SPAWN  → Normal monster wave
├── ELITE_MONSTER_SPAWN   → Elite monster wave
├── BOSS_MONSTER_SPAWN    → Boss monster wave
├── GAME_OVER             → Player HP reaches 0
└── GAME_CLEAR            → All 9 waves cleared
```

---

## Architecture

### Design Patterns

- **Singleton** — `GameManager` and `GoldManager` persist across scene loads via `DontDestroyOnLoad`
- **Observer (Unity Events)** — `OnAgeChanged`, `OnGoldChanged`, `IsGameStateBeforeGateOpen`, etc. enable loose coupling between systems
- **Polymorphism** — Abstract base classes `Monster` and `Turret` separate shared logic from per-type behavior
- **Finite State Machine (FSM)** — `GameManager` drives the game loop through a `GameFlowState` enum. State transitions are triggered by the current **wave number** and **phase**, as follows:

  ```
  Wave number  →  Phase  →  GameFlowState transition
  ─────────────────────────────────────────────────────
  Wave  1 / 4 / 7  (wave % 3 == 1)  →  NORMAL_MONSTER_SPAWN
  Wave  2 / 5 / 8  (wave % 3 == 2)  →  ELITE_MONSTER_SPAWN
  Wave  3 / 6 / 9  (wave % 3 == 0)  →  BOSS_MONSTER_SPAWN
  Phase change  (every 3 waves)      →  OnAgeChanged event  →  GameAge advances
  All 9 waves complete               →  GAME_CLEAR
  Player HP ≤ 0                      →  GAME_OVER (from any state)
  ```

  Each state controls which spawner is active, what stats are applied (via `phase = (wave - 1) / 3`), and which UI elements are visible. `BEFORE_GATE_OPEN` is a transient prep state entered between waves, giving players a window to place or upgrade turrets before the next spawn begins.

### Script Structure

```
Assets/02_Scripts/
├── Game/
│   ├── GameManager.cs          # Game flow, wave progression, event broadcasting
│   ├── GameFlowState.cs        # Game state enum
│   └── GameAge.cs              # Era enum
├── PlaneTracking/
│   ├── GameBoardGenerator.cs   # AR plane detection, tile grid generation, board confirm UI
│   └── Tile.cs                 # Individual tile (occupancy, installed turret reference)
├── Monster/
│   ├── Monster.cs              # Abstract base: HP, damage, death/reward handling
│   ├── NormalMonster.cs
│   ├── EliteMonster.cs
│   ├── BossMonster.cs
│   └── MonsterStats.cs         # Stat constants and scaling formulas
├── Turret/
│   ├── Turret.cs               # Abstract base: target detection (OverlapSphere), attack, upgrade
│   ├── MiddleAgeTurret.cs
│   ├── ModernAgeTurret.cs
│   ├── FutureAgeTurret.cs
│   ├── TurretInstaller.cs      # Touch input → tile/turret raycast → placement/selection
│   ├── TurretStats.cs
│   └── TurretCost.cs
├── EnemySpawn/
│   ├── EnemySpawner.cs         # Wave-based spawning and phase transitions
│   ├── EnemyAI.cs              # Pathfinding, nearest-turret targeting, player pursuit
│   └── GateController.cs       # Chaos Gate open/close animation
├── Gold/
│   ├── GoldManager.cs          # Singleton, balance management, event publishing
│   └── Gold.cs                 # Reward constants
└── UI/
    ├── MainUIManager.cs
    └── GameUIManagerScript1.cs
```

### Key Technical Points

1. **AR Plane Validation** — Only planes with `HorizontalUp` alignment that meet the minimum grid size are accepted for board placement
2. **Board Rotation Fitting** — Both standard and 90°-rotated orientations are evaluated; the better fit is selected automatically
3. **`Physics.OverlapSphere`** — Used each attack cycle for efficient spherical target detection around each turret
4. **Coroutine-Based Spawning** — Spawn intervals and wave transitions are controlled via coroutines for precise timing

---

## Getting Started

### Requirements

- Unity 6000.3.10f or later
- AR Foundation package
- Android: ARCore-supported device / iOS: ARKit-supported device (iOS 11+)

### Build Steps

1. Clone this repository.
   ```bash
   git clone https://github.com/2026-1-IMP-Team1/ARDefense.git
   ```
2. Open the project in Unity Hub.
3. In **Build Settings**, set the target platform to Android or iOS.
4. Under **Player Settings → XR Plug-in Management**, enable **ARCore** (Android) or **ARKit** (iOS).
5. Build and deploy to a physical device.

### How to Play

1. Launch the app and slowly move the camera toward a flat horizontal surface such as a floor or desk.
2. Once a plane is detected, a confirmation dialog appears — tap **Yes** to generate the game board.
3. Tap tiles on the board to place turrets.
4. Waves begin automatically. Survive all 9 waves to win!

---

> 2026 Spring Semester — IMP (Immersive Media Programming) Midterm Project