namespace Collections;

public class JobSelectorWidget
{
    public Dictionary<ClassJob, bool> Filters;

    private const int JobIconScale = 7;
    private const int IconSize = 25;
    private Vector2 overrideItemSpacing = new(2, 1);

    // Specific filter for "All Classes" items
    private bool allClasses = true;

    private EventService EventService { get; init; }
    public JobSelectorWidget(EventService eventService)
    {
        EventService = eventService;
        var classJobs = Services.DataProvider.SupportedClassJobs;
        Filters = classJobs.ToDictionary(entry => entry, entry => true);
        roles = classJobs.GroupBy(entry =>
        {
            // UI Priority to the rescue
            // specifically want to truncate anything below 10
            // This will probably have to be updated to be more complex
            // once they introduce a new melee class (Viper is at UI prio 39 atm)
            return (uint)entry.UIPriority / 10;
        }).OrderBy(entry => entry.Key).ToDictionary(entry => entry.Key, entry => entry.OrderBy(job => job.UIPriority).ToList());
    }

    public void Draw()
    {
        // Draw Buttons
        ImGui.PushStyleColor(ImGuiCol.Button, Services.WindowsInitializer.MainWindow.originalButtonColor);
        if (ImGui.Button("Enable All"))
        {
            SetAllState(true, true);
        }
        ImGui.SameLine();
        if (ImGui.Button("Disable All"))
        {
            SetAllState(false, true);
        }
        ImGui.SameLine();
        if (ImGui.Button("Current Job"))
        {
            SetCurrentJob();
        }
        ImGui.PopStyleColor();

        // Draw job icons
        JobSelector();
    }

    private Dictionary<uint, List<ClassJob>> roles;
    private void JobSelector()
    {
        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, overrideItemSpacing);
        foreach (var classes in roles.Values)
        {
            foreach (var job in classes)
            {
                var icon = job.GetIcon();
                if (icon != null)
                {
                    // Button
                    UiHelper.ImageToggleButton(icon, new Vector2(IconSize, IconSize), Filters[job]);

                    // Left click - Switch to selection
                    if (ImGui.IsItemClicked(ImGuiMouseButton.Left))
                    {
                        var newState = IsAllActive() ? true : !Filters[job];
                        SetAllState(false, false);
                        Filters[job] = newState;
                        PublishFilterChangeEvent();
                    }

                    // Right click - Toggle selection
                    if (ImGui.IsItemClicked(ImGuiMouseButton.Right))
                    {
                        Filters[job] = !Filters[job];
                        PublishFilterChangeEvent();
                    }
                }
                ImGui.SameLine();
            }
            // lets us not have to do logic around ImGui.SameLine()
            ImGui.Text("");
        }
        // "All Classes" Icon Button
        UiHelper.ImageToggleButton(IconHandler.GetIcon(62522), new Vector2(IconSize, IconSize), allClasses);
        if (ImGui.IsItemClicked(ImGuiMouseButton.Left))
        {
            var newState = IsAllActive() ? true : !allClasses;
            SetAllState(false, false);
            allClasses = newState;
            PublishFilterChangeEvent();
        }
        if (ImGui.IsItemClicked(ImGuiMouseButton.Right))
        {
            allClasses = !allClasses;
            PublishFilterChangeEvent();
        }
        ImGui.PopStyleVar();
    }

    private void SetAllState(bool state, bool publishEvent)
    {
        Filters = Filters.ToDictionary(e => e.Key, e => state);
        allClasses = state;
        if (publishEvent)
            PublishFilterChangeEvent();
    }

    private void SetCurrentJob()
    {
        var matchingClassJob = Filters.Where(e => e.Key.RowId == Services.ClientState.LocalPlayer.ClassJob.RowId);
        if (matchingClassJob.Any())
        {
            SetAllState(false, false);
            Filters[matchingClassJob.First().Key] = true;
            PublishFilterChangeEvent();
        }
    }

    private bool IsAllActive()
    {
        return !Filters.Where(e => e.Value == false).Any() && allClasses;
    }

    public bool AllClasses()
    {
        return allClasses;
    }

    private void PublishFilterChangeEvent()
    {
        EventService.Publish<FilterChangeEvent, FilterChangeEventArgs>(new FilterChangeEventArgs());
    }
}

