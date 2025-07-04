// Holds user-related data used throughout the app.
// This is populated on login and can be updated as the user progresses.
[System.Serializable]
public class UserData
{
    public string _id;           // MongoDB internal ID (optional)
    public string id;            // Your own student/teacher ID (required for this feature ✅)
    public string firstName;
    public string lastName;
    public string username;
    public string password;
    public int userType;
    public int totalPoints;
    public ShapeStats shapes;
    public bool isLoggedIn;
}

// Stores points per shape type for educational performance tracking
[System.Serializable]
public class ShapeStats
{
    public int triangle;
    public int square;

}
