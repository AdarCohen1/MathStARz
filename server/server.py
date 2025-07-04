from fastapi import FastAPI
from routes import router
from fastapi.middleware.cors import CORSMiddleware

# Initialize the FastAPI app
app = FastAPI()

# Include your API router from routes.py
app.include_router(router)

# Root route for health check or basic test
@app.get("/")
def read_root():
    return {"message": "MathStarz API is live!"}

# CORS middleware for allowing requests from any origin (Unity, web, etc.)
app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],  # You can restrict this to your frontend domain if needed
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)


# Render automatically runs: uvicorn server:app --host=0.0.0.0 --port=10000
