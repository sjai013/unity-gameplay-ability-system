using TMPro;

public class GameplayTagStatusBarButton : GenericUIIcon {
    public TextMeshProUGUI TextMeshPro;

    public void SetStacks(int stacks) {
        var stacksString = "";
        if (stacks > 1) stacksString = stacks.ToString();
        TextMeshPro.text = stacksString;
    }
}