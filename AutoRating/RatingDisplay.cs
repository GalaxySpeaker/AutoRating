using AdofaiRater;
using AutoRating.AdofaiRater;
using UnityEngine;

public class RatingDisplay : MonoBehaviour
{
    public float positionX = 0f;
    public float positionY = 0.3f;
    public float fontSize = 30;

    private void OnGUI()
    {
        GUIStyle labelStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = (int)fontSize,  //  Main.cs 的字体大小
            alignment = TextAnchor.UpperLeft,
            normal = { textColor = Color.white }
        };

        string globalRating = RaterMain.globalRating.ToString("F2");
        string finalScore = CalculatePP.finalScore.ToString("F0");
        string PP = CalculatePP.PP.ToString("F2");
        string maxPP = CalculatePP.maxPP.ToString("F2");

        GUI.Label(new Rect(positionX * Screen.width, positionY * Screen.height, 1000, 20), "Star Rating: " + globalRating, labelStyle);
        GUI.Label(new Rect(positionX * Screen.width, (positionY + fontSize / 1000) * Screen.height, 1000, 20), "Final Score: " + finalScore, labelStyle);
        GUI.Label(new Rect(positionX * Screen.width, (positionY + 2 * fontSize / 1000) * Screen.height, 1000, 20), "PP: " + PP, labelStyle);
        GUI.Label(new Rect(positionX * Screen.width, (positionY + 3 * fontSize / 1000) * Screen.height, 1000, 20), "Max PP: " + maxPP, labelStyle);
    }
}
