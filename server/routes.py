from fastapi import APIRouter, HTTPException, Body,UploadFile, File
from models import UserLogin, ScoreUpdate, UserRegister, FullUserData, PuzzleProgress
from fastapi import Query
from database import find_user_by_username, users_collection, get_top_users, update_puzzle_progress, get_user_puzzle, get_all_puzzles_by_user
from logic.user_logic import *


router = APIRouter()

@router.post("/users/register")
async def register(user: UserRegister):
    success, message = handle_register(user)
    if not success:
        raise HTTPException(status_code=400, detail=message)
    return {"status": "success", "message": message}


@router.post("/users/login")
async def login(user: UserLogin):
    user_in_db = handle_login(user)
    if user_in_db:
        if "_id" in user_in_db:
            user_in_db["_id"] = str(user_in_db["_id"])
                # ✅ Set isLoggedIn to True in DB
            users_collection.update_one(
                {"username": user.username},
                {"$set": {"isLoggedIn": True}}
            )
        return {
            "status": "success",
            "message": "Login successful",
            "user": user_in_db
        }
    
    raise HTTPException(status_code=401, detail="Invalid credentials")

@router.post("/users/logout")
async def logout(username: str = Body(...)):
    result = users_collection.update_one(
        {"username": username},
        {"$set": {"isLoggedIn": False}}
    )
    if result.modified_count > 0:
        return {"status": "success", "message": "User logged out"}
    else:
        raise HTTPException(status_code=404, detail="User not found")

@router.get("/users/exists")
async def user_exists(username: str = Query(...)):
    user = find_user_by_username(username)
    return str(bool(user)).lower()

@router.post("/users/verify-password")
async def verify_password(user: UserLogin):
    match = find_user_by_credentials(user.username, user.password)
    return str(bool(match)).lower()

@router.get("/users/is-logged-in")
async def is_logged_in(username: str = Query(...)):
    user = find_user_by_username(username)
    return str(user.get("isLoggedIn", False)).lower() if user else "false"


@router.get("users/leaderboard")
async def leaderboard():
    return handle_leaderboard()

@router.get("/questions/{qid}")
async def get_question(qid: str):
    result = handle_get_question(qid)
    if "error" in result:
        raise HTTPException(status_code=404, detail=result["error"])
    return result

@router.get("/users")
async def get_user(id: str = Query(None), username: str = Query(None)):
    if id:
        user = handle_find_user_by_id(id)
    elif username:
        user = find_user_by_username(username)
    else:
        raise HTTPException(status_code=400, detail="Missing id or username")

    if not user:
        raise HTTPException(status_code=404, detail="User not found")

    user["_id"] = str(user["_id"])
    return user



@router.post("/users/update")
async def update_userdata(user: FullUserData):
    handle_user_update(user.dict())
    return {"status": "success", "message": "User data updated"}


@router.get("/users/leaderboard")  # Important: add leading slash!
async def leaderboard():
    return get_top_users(5)


@router.post("/puzzles/update")
async def update_puzzle(puzzle: PuzzleProgress):
    update_puzzle_progress(puzzle.userId, puzzle.puzzleId, puzzle.piecesCollected)
    return {"status": "success", "message": "Puzzle progress updated"}

@router.get("/puzzles/get")
async def get_puzzle(userId: int, puzzleId: int):
    puzzle = get_user_puzzle(userId, puzzleId)
    if puzzle:
        return puzzle
    raise HTTPException(status_code=404, detail="Puzzle not found")

@router.get("/puzzles/user")
async def get_all_user_puzzles(userId: int):
    return get_all_puzzles_by_user(userId)


@router.get("/users/check-loggedin")
async def check_logged_in(id: str = Query(...)):
    user = handle_check_logged_in(id)
    if user:
        user["_id"] = str(user["_id"])
        return {"status": "loggedin", "user": user}
    raise HTTPException(status_code=404, detail="User not logged in")
