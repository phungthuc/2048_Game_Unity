using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenManager : MonoBehaviour
{
    [SerializeField] private List<ScreenController> _listScreens = new List<ScreenController>();

    public void ShowScreen(string screenName)
    {
        HideAllScreen();
        var screen = FindScreenName(screenName);

        if (screen == null)
        {
            Debug.LogError("Can't Find Screen Name: " + screenName);
            return;
        }
        screen.Show();
    }

    public void HideAllScreen()
    {
        foreach (var item in _listScreens)
        {
            item.Hide();
        }
    }

    public ScreenController FindScreenName(string name)
    {
        return _listScreens.Find(item => item.ScreenName.Equals(name));
    }
}
