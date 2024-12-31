using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CheatManager : MonoBehaviour {
    [SerializeField] private TMP_Dropdown storylinesDropdown;
    [SerializeField] private TMP_Dropdown timelinesDropdown;
    [SerializeField] private Button storylinesButton;
    [SerializeField] private TextMeshProUGUI activeStorylineText;
    [SerializeField] private Toggle stopStorylineToggle;

    private void OnEnable() {
        activeStorylineText.text = "";
        var activeLaunchers = Checker.Instance.activeLaunchers;
        if (activeLaunchers.Count == 0) {
            activeStorylineText.text = "No active storyline";
            return;
        }

        activeStorylineText.text = "Active storylines:\n";
        for (var index = 0; index < activeLaunchers.Count; index++) {
            var activeLauncher = activeLaunchers[index];
            activeStorylineText.text +=
                $"- Storyline : {activeLauncher.storyline.StorylineContainer.FileName} / Timeline : {activeLauncher.timeline.TimelineContainer.GroupName}\n";
        }
    }

    public void Initialize() {
        storylinesDropdown.options.Clear();
        var storylines = Checker.Instance.allStorylines;
        for (var index = 0; index < storylines.Count; index++) {
            var storyline = storylines[index];
            storylinesDropdown.options.Add(new TMP_Dropdown.OptionData(storyline.StorylineContainer.FileName));
        }
        storylinesDropdown.RefreshShownValue();

        storylinesDropdown.onValueChanged.AddListener(OnStorylineSelected);
        storylinesButton.onClick.AddListener(LaunchStoryline);
        stopStorylineToggle.onValueChanged.AddListener(StopChecker);
    }

    private void OnStorylineSelected(int index) {
        timelinesDropdown.options.Clear();
        var storyline = Checker.Instance.allStorylines.First(storyline =>
            storyline.StorylineContainer.FileName == storylinesDropdown.options[index].text);
        for (int i = 0; i < storyline.StorylineContainer.NodeGroups.Count; i++) {
            var timeline = storyline.StorylineContainer.NodeGroups.Keys.ElementAt(i);
            timelinesDropdown.options.Add(new TMP_Dropdown.OptionData(timeline.GroupName));
        }
        timelinesDropdown.RefreshShownValue();
    }

    private void LaunchStoryline() {
        var storyline = Checker.Instance.allStorylines.First(storyline =>
            storyline.StorylineContainer.FileName == storylinesDropdown.options[storylinesDropdown.value].text);
        var timeline = storyline.StorylineContainer.NodeGroups.Keys.ElementAt(timelinesDropdown.value);
        Checker.Instance.StartTimeline(storyline, timeline);
    }

    private void StopChecker(bool value) {
        Checker.Instance.IsStopChecker = value;
    }
}