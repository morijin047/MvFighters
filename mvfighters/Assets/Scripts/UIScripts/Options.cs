using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Option", menuName = "Option / Add")]
public class Options : ScriptableObject
{
    public bool isSubMenu;
    public MenuSelection option;
    public VersusSelection versusOption;
    public OnlineSelection onlineOption;
    public TrainingSelection trainingOption;
    public OptionSelection optionsOption;
}
