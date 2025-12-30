using System.Security.Cryptography;

namespace Collections;

public class GlamourTab : IDrawable
{
    private List<ICollectible> filteredCollection { get; set; }
    private GlamourTreeWidget GlamourTreeWidget { get; init; }
    private JobSelectorWidget JobSelectorWidget { get; init; }
    private ContentFiltersWidget ContentFiltersWidget { get; init; }
    private EquipSlotsWidget EquipSlotsWidget { get; init; }
    private CollectionWidget CollectionWidget { get; init; }
    private EventService EventService { get; init; }
    private bool FiltersCollapsed { get; set; } = false;

    public GlamourTab()
    {
        EventService = new EventService();
        GlamourTreeWidget = new GlamourTreeWidget(EventService);
        JobSelectorWidget = new JobSelectorWidget(EventService);
        ContentFiltersWidget = new ContentFiltersWidget(EventService, 2);
        EquipSlotsWidget = new EquipSlotsWidget(EventService);
        filteredCollection = GetInitialCollection();
        CollectionWidget = new CollectionWidget(EventService, true, filteredCollection.Count > 0 ? filteredCollection.First().GetSortOptions() : null);

        ApplyFilters();

        EventService.Subscribe<FilterChangeEvent, FilterChangeEventArgs>(OnPublish);
        EventService.Subscribe<GlamourItemChangeEvent, GlamourItemChangeEventArgs>(OnPublish);
        EventService.Subscribe<GlamourSetChangeEvent, GlamourSetChangeEventArgs>(OnPublish);
        EventService.Subscribe<ReapplyPreviewEvent, ReapplyPreviewEventArgs>(OnPublish);

        // GlamourTreeWidget will always have at least one Directory + Set.
        // Therefore once everything is initialized, set selection to first set (0, 0)
        GlamourTreeWidget.SetSelectedGlamourSet(0, 0, false);
    }

    private const int GlamourSetsWidgetWidth = 15;
    private const int SpaceBetweenFilterWidgets = 3;

    public void Draw()
    {
        // Dev.Start();

        if (ImGui.BeginTable("glam-tree", 1, ImGuiTableFlags.Borders | ImGuiTableFlags.NoHostExtendX | ImGuiTableFlags.SizingFixedFit))
        {

            ImGui.TableSetupColumn("Sets", ImGuiTableColumnFlags.None, UiHelper.UnitWidth() * GlamourSetsWidgetWidth);
            ImGui.TableHeadersRow();
            // if (ImGuiComponents.IconButton(FontAwesomeIcon.ArrowLeft))
            // {
            //     GlamourTreeCollapsed = !GlamourTreeCollapsed;
            // }

            // if (!GlamourTreeCollapsed)
            // {
            ImGui.TableNextRow(ImGuiTableRowFlags.None, UiHelper.GetLengthToBottomOfWindow());
            ImGui.TableNextColumn();

            GlamourTreeWidget.Draw();
            // }

            ImGui.EndTable();
        }
        ImGui.SameLine();

        // Equip slot buttons
        ImGui.BeginGroup();
        if (ImGui.BeginTable("equip-slots", 1, ImGuiTableFlags.Borders | ImGuiTableFlags.NoHostExtendX | ImGuiTableFlags.SizingFixedFit))
        {
            ImGui.TableSetupColumn("Equip Slots", ImGuiTableColumnFlags.None); // Not setting width here, allowing equip slot icon size to dictate width
            ImGui.TableHeadersRow();

            ImGui.TableNextRow(ImGuiTableRowFlags.None, UiHelper.GetLengthToBottomOfWindow());
            ImGui.TableNextColumn();
            EquipSlotsWidget.Draw();

            ImGui.EndTable();
        }
        ImGui.EndGroup();
        ImGui.SameLine();

        // Filters
        ImGui.BeginGroup();
        if (ImGui.BeginTable("filters", 1, ImGuiTableFlags.Borders | ImGuiTableFlags.NoHostExtendX | ImGuiTableFlags.SizingFixedSame))
        {

            ImGui.TableNextColumn();
            // ImGui.TableHeader("Filters");

            // Draw button as a group
            UiHelper.GroupWithMinWidth(() =>
            {
                var cursorPos = ImGui.GetCursorPos();
                ImGui.TableHeader("");
                ImGui.SetCursorPos(new Vector2(cursorPos.X + 2, cursorPos.Y));
                if (!FiltersCollapsed)
                {
                    ImGuiComponents.IconButton(FontAwesomeIcon.ArrowLeft);
                    ImGui.SameLine();
                    ImGui.Text("Filter");
                }
                else
                {
                    ImGuiComponents.IconButton(FontAwesomeIcon.ArrowRight);
                }
            }, 5);
            // var cursorPos = ImGui.GetCursorPos();
            // ImGui.TableHeader("");
            // ImGui.SetCursorPos(new Vector2(cursorPos.X + 5, cursorPos.Y));
            // if (ImGuiComponents.IconButton(FontAwesomeIcon.Filter, new Vector2(13, 13)))
            if (ImGui.IsItemClicked())
            {
                FiltersCollapsed = !FiltersCollapsed;
            }

            if (!FiltersCollapsed)
            {
                // ImGui.TableNextRow();
                // ImGui.TableNextColumn();
                // ImGui.Text("Jobs");
                // if (ImGui.IsItemClicked())
                // {
                //     FiltersCollapsed = !FiltersCollapsed;
                // }

                ImGui.TableNextRow(ImGuiTableRowFlags.None, (UiHelper.GetLengthToBottomOfWindow() / 2) - (UiHelper.UnitHeight() * SpaceBetweenFilterWidgets));
                ImGui.TableNextColumn();
                JobSelectorWidget.Draw();

                ImGui.TableNextRow();
                ImGui.TableNextColumn();
                // ImGui.TableHeader("Content Filters");
                ImGui.Text("Source");

                ImGui.TableNextRow(ImGuiTableRowFlags.None, UiHelper.GetLengthToBottomOfWindow());
                ImGui.TableNextColumn();
                ContentFiltersWidget.Draw();
            }
            ImGui.EndTable();
        }

        ImGui.EndGroup();

        // Glam collection
        ImGui.SameLine();
        ImGui.BeginGroup();

        CollectionWidget.Draw(filteredCollection);
        //ImGui.Text("");

        ImGui.EndGroup();

        //var drawTime = Dev.Stop(false);
        //DrawTimer(drawTime);
    }

    private double drawCount = 1;
    private double drawAvg = 0;
    private void DrawTimer(double drawTime)
    {
        drawAvg = drawAvg + (drawTime - drawAvg) / drawCount;
        drawCount++;
        ImGui.Text(drawAvg.ToString());

        ImGui.SameLine();
        if (ImGui.Button("Reset"))
        {
            drawCount = 1;
            drawAvg = 0;
        }

        if (drawCount > 1000)
        {
            drawCount = 1;
            drawAvg = 0;
        }
        ImGui.Text(" ");
    }

    public void OnOpen()
    {
        Dev.Log();

        Task.Run(() =>
        {
            foreach (var collectible in Services.DataProvider.GetCollection<GlamourCollectible>())
            {
                collectible.UpdateObtainedState();
            }
        });
    }

    List<ICollectible> GetInitialCollection() => Services.DataProvider.GetCollection<GlamourCollectible>();

    private void ApplyFilters()
    {
        // Refresh all filters (1) Equip slot (2) content type (3) job
        var contentFilters = ContentFiltersWidget.Filters.Where(d => d.Value).Select(d => d.Key);
        var jobFilters = JobSelectorWidget.Filters.Where(d => d.Value).Select(d => d.Key).ToList();

        // (1) Equip Slot filter
        filteredCollection = CollectionWidget.PageSortOption.SortCollection(GetInitialCollection())
            .Where(c => ((GlamourCollectible)c).ExcelRow.GetEquipSlot() == (EquipSlotsWidget.activeEquipSlot == EquipSlot.FingerL ? EquipSlot.FingerR : EquipSlotsWidget.activeEquipSlot))
        // (2) Content type filters
        .Where(c => c.CollectibleKey is not null)
        .Where(c => !contentFilters.Any() || contentFilters.Intersect(c.CollectibleKey.SourceCategories).Any())
        // (3) job filters
        .Where(c =>
            {
                // show all items if all filters disabled
                if (!jobFilters.Any() && !JobSelectorWidget.AllClasses())
                    return true;
                var itemJobCat = ((GlamourCollectible)c).ExcelRow.ClassJobCategory.Value;
                // only show "All Classes" items if toggled
                if (itemJobCat.RowId < 2) return JobSelectorWidget.AllClasses();
                var itemJobs = itemJobCat.GetJobs();
                foreach (var jobFilter in jobFilters)
                {
                    if (itemJobs.Contains(jobFilter))
                    {
                        return true;
                    }
                }
                return false;
            })
        // Order
        .Where(c => !CollectionWidget.IsFiltered(c))
        .ToList();
    }

    public void OnPublish(GlamourItemChangeEventArgs args)
    {
        var equipSlot = args.Collectible.ExcelRow.GetEquipSlot();
        if (equipSlot == EquipSlot.FingerR && EquipSlotsWidget.activeEquipSlot == EquipSlot.FingerL)
        {
            Dev.Log("Left Hand Ring Published");
            equipSlot = EquipSlotsWidget.activeEquipSlot;
        }
        var stain0Id = EquipSlotsWidget.paletteWidgets[equipSlot].ActiveStainPrimary.RowId;
        var stain1Id = EquipSlotsWidget.paletteWidgets[equipSlot].ActiveStainSecondary.RowId;
        Services.PreviewExecutor.PreviewWithTryOnRestrictions(args.Collectible, stain0Id, stain1Id, Services.Configuration.ForceTryOn, equipSlot);
    }

    public void OnPublish(GlamourSetChangeEventArgs args)
    {
        // On some events (during initializing) we're suppressing preview
        if (!args.Preview)
            return;

        // Reset glamour state. TODO reset Try On
        Services.PreviewExecutor.ResetAllPreview();

        // Preview the selected set
        foreach (var (equipSlot, glamourItem) in args.GlamourSet.Items)
        {
            Services.PreviewExecutor.PreviewWithTryOnRestrictions(glamourItem.GetCollectible(), glamourItem.Stain0Id, glamourItem.Stain1Id, false, equipSlot);
        }
    }

    public void OnPublish(FilterChangeEventArgs args)
    {
        ApplyFilters();
    }

    public void OnPublish(ReapplyPreviewEventArgs args)
    {
        Services.PreviewExecutor.ResetAllPreview();
        foreach (var (equipSlot, glamourItem) in EquipSlotsWidget.currentGlamourSet.Items)
        {
            var collectible = CollectibleCache<GlamourCollectible, Item>.Instance.GetObject(glamourItem.ItemId);
            Services.PreviewExecutor.PreviewWithTryOnRestrictions(collectible, glamourItem.Stain0Id, glamourItem.Stain1Id, Services.Configuration.ForceTryOn, equipSlot);
        }
    }

    public void Dispose()
    {
    }

    public void OnClose()
    {
        GlamourTreeWidget.SaveGlamourTreeToConfiguration();
    }
}

