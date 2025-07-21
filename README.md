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

📸 Game Screenshots
1. AR World Exploration

<img width="563" height="280" alt="image" src="https://github.com/user-attachments/assets/3c907183-4b57-4e91-b838-ad0dd0922844" />

Students explore an interactive AR world with NPCs, wooden bridges, and objects that appear after scanning geometric shapes.

2. Main Menu & Leaderboard

<img width="580" height="264" alt="image" src="https://github.com/user-attachments/assets/119d63cf-34e5-4224-8a3a-f435a0b97c1d" />

A multilingual main menu displaying leaderboards, player scores, sound settings, and language selection.

3. Geometry Question Example


<img width="549" height="278" alt="image" src="https://github.com/user-attachments/assets/44bdc13f-00f0-4bf8-a0f8-f952f3dba741" />


Students answer interactive geometry-related multiple-choice questions, such as calculating the volume of a cube.

4. Teacher Admin Panel – Add Student

<img width="560" height="266" alt="image" src="https://github.com/user-attachments/assets/d3dae97c-d86f-47a5-8f4d-c06060c48139" />


Teachers and admins can register new students directly through the admin panel.

5. Puzzle Progress Tracking

<img width="588" height="274" alt="image" src="https://github.com/user-attachments/assets/40fdddf2-59f9-4a39-837d-7b8ffea1613b" />

Correct answers unlock puzzle pieces, allowing students to visually track their progress.









