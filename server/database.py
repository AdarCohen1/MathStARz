from pymongo import MongoClient
from config import MONGO_URI

client = MongoClient(MONGO_URI)
db = client["mathstarzdb"]
users_collection = db["users"]
questions_collection = db["questions"]
puzzles_collection = db["puzzles"]

def insert_user(user_dict):
    return users_collection.insert_one(user_dict)

def find_user_by_username(username):
    return users_collection.find_one({"username": username})

def find_user_by_credentials(username, password):
    return users_collection.find_one({"username": username, "password": password})

def get_user_score(username):
    user = users_collection.find_one({"username": username}, {"totalPoints": 1})
    return user.get("totalPoints", 0) if user else 0

def update_user_score(username, score):
    return users_collection.update_one(
        {"username": username},
        {"$set": {"totalPoints": score}},
        upsert=True
    )

def get_top_users(limit=10):
    return list(users_collection.find({}, {"username": 1, "totalPoints": 1, "_id": 0}).sort("totalPoints", -1).limit(limit))

def get_question_by_id(qid):
    return questions_collection.find_one({"_id": qid}, {"_id": 0})

def update_full_user(user_data):
    return users_collection.update_one(
        {"username": user_data["username"]},
        {"$set": user_data}
    )

def get_top_users(limit=10):
    return list(users_collection.find({}, {"username": 1, "totalPoints": 1, "_id": 0})
                .sort("totalPoints", -1)
                .limit(limit))

# Insert or update puzzle progress
def update_puzzle_progress(user_id: int, puzzle_id: int, pieces: int):
    return puzzles_collection.update_one(
        {"userId": user_id, "puzzleId": puzzle_id},
        {"$set": {"piecesCollected": pieces}},
        upsert=True
    )

# Get progress for a specific puzzle & user
def get_user_puzzle(user_id: int, puzzle_id: int):
    return puzzles_collection.find_one({"userId": user_id, "puzzleId": puzzle_id}, {"_id": 0})

# Get all puzzle progress for a user
def get_all_puzzles_by_user(user_id: int):
    return list(puzzles_collection.find({"userId": user_id}, {"_id": 0}))

def find_logged_in_user_by_id(user_id):
    return users_collection.find_one({"id": user_id, "isLoggedIn": True})

def find_user_by_id(user_id):
    return users_collection.find_one({"id": user_id})

