# MathStarz AR: Educational Puzzle Game with Unity & FastAPI

**MathStarz AR** is an interactive Augmented Reality (AR) educational game built with Unity and powered by a FastAPI backend. It helps middle and high school students practice geometry through shape recognition, localized questions, and puzzle-based rewards.

## 🎮 Gameplay Overview

- Scan a **real-world geometric shape** (e.g. cube, pyramid).
- Enter an AR world based on that shape.
- Interact with NPCs who ask geometry questions.
- Answer correctly to **earn puzzle pieces** and score points.
- Complete puzzles to unlock new content and track progress!

## ✨ Features

### 🔷 Unity (Frontend)
- 📱 **AR interaction** using Vuforia.
- 👤 **Multilingual support**: English + Hebrew (with RTL UI handling).
- 🗨️ **Dialog system** for NPC interactions.
- ❓ **Open and Multiple Choice Questions** with dynamic UI.
- 🧩 **Puzzle progress tracking** per student.
- 🔊 **Audio cues** and volume control.
- 🧑‍🏫 Separate UIs for students, teachers, and admins.
- 📈 Leaderboards and user performance stats.

### 🔶 FastAPI (Backend)
- 🧑 **User registration/login** with MongoDB (students/teachers/admins).
- 🔒 Auto-login & session tracking.
- ❓ **Question delivery API** with image & answer support.
- 🧠 **Puzzle progress API** for saving and syncing.
- 📊 **Leaderboard API** for top players.

## 🗃️ Tech Stack

### Unity (Frontend)
