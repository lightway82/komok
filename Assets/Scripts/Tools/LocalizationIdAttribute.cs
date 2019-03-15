using UnityEngine;

public class LocalizationIdAttribute : PropertyAttribute {
    public readonly bool DisplayMenu;
    public LocalizationIdAttribute (bool displayMenu = false) {
        DisplayMenu = displayMenu;
    }
}