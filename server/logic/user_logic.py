from database import (
    insert_user, find_user_by_username, find_user_by_credentials,
    get_user_score, update_user_score, get_top_users,
    get_question_by_id
)
from database import *

def handle_register(user):
    if find_user_by_username(user.username):
        return False, "Username already exists"

    user_dict = {
        "id": user.id, 
        "firstName": user.firstName,
        "lastName": user.lastName,
        "username": user.username,
        "password": user.password,
        "userType": user.userType,
        "totalPoints": 0,
        "shapes": {"triangle": 0, "square": 0, "circle": 0},
        "isLoggedIn": False
    }


    insert_user(user_dict)
    return True, "User registered successfully"


def handle_login(user):
    return find_user_by_credentials(user.username, user.password)

def handle_score_update(username, score):
    current = get_user_score(username)
    new_score = current + score
    update_user_score(username, new_score)
    return new_score

def handle_leaderboard():
    return get_top_users()

def handle_get_question(qid):
    question = get_question_by_id(qid)
    return question or {"error": "Question not found"}

def handle_user_update(user_data):
    return update_full_user(user_data)

def handle_check_logged_in(user_id):
    return find_logged_in_user_by_id(user_id)

def handle_find_user_by_id(user_id):
    return find_user_by_id(user_id)


