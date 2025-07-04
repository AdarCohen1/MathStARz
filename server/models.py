from pydantic import BaseModel

class UserRegister(BaseModel):
    id: str  
    firstName: str
    lastName: str
    username: str
    password: str
    userType: int


class UserLogin(BaseModel):
    username: str
    password: str

class ScoreUpdate(BaseModel):
    username: str
    score: int

class FullUserData(BaseModel):
    _id: str
    firstName: str
    lastName: str
    username: str
    password: str
    userType: int
    totalPoints: int
    shapes: dict
    isLoggedIn: bool

class PuzzleProgress(BaseModel):
    puzzleId: int      # numeric puzzle ID
    userId: int        # numeric user ID
    piecesCollected: int