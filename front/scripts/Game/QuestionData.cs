
// Represents a single question's data structure, used throughout the game.
// This class is populated from JSON responses coming from the server.
[System.Serializable]
public class QuestionData
{
    public string shape;
    public string question_text;
    public string question_type;
    public string answer;
    public string[] options;
    public int correct_option_index;
    public string image_filename;
    public int points;
}
