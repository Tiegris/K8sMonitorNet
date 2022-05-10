using K8sMonitorCore.Aggregation.Dto.Simple;
using K8sMonitorCore.Aggregation.Dto.Tree;
using K8sMonitorCore.Aggregation.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;

namespace WebUi.Pages;

public class IndexModel : PageModel
{
    private readonly AggregationService aggregation;

    public IndexModel(AggregationService aggregation) {
        this.aggregation = aggregation;
    }

    public IEnumerable<NodeNsDto>? StatusList { get; set; }

    public ICollection<SelectListItem> GroupByOptions { get; set; } = new SelectListItem[] {
            new SelectListItem {
                Text = "TreeView",
                Value = "tv"
            },
            new SelectListItem {
                Text = "Service",
                Value = "srv"
            },
            new SelectListItem {
                Text = "Namespace",
                Value = "ns"
            },
        };



    public void OnGet() {
        StatusList = aggregation.TreeGrouping();
    }


    [BindProperty]
    public string? GroupById { get; set; }

    public PartialViewResult OnPost() {
        if (GroupById is "tv")
            return Partial("_TreeView", aggregation.TreeGrouping());

        IEnumerable<SimpleStatusDto> lst;
        if (GroupById is "ns")
            lst = aggregation.GetHealthGroupByNs();
        else if (GroupById is "srv")
            lst = aggregation.GetHealthGroupBySrv();
        else
            throw new ArgumentException($"Invalid GroupById: {GroupById}");

        return Partial("_SimpleList", lst);
    }
}
