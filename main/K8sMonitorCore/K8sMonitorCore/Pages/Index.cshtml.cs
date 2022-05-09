using K8sMonitorCore.Aggregation.Dto.Simple;
using K8sMonitorCore.Aggregation.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace WebUi.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly AggregationService aggregation;

    public IndexModel(ILogger<IndexModel> logger, AggregationService aggregation) {
        _logger = logger;
        this.aggregation = aggregation;
    }

    public IEnumerable<SimpleStatusDto>? StatusList { get; set; }

    public ICollection<SelectListItem> GroupByOptions { get; set; } = new SelectListItem[] {
            new SelectListItem {
                Text = "Service",
                Value = "srv"
            },
            new SelectListItem {
                Text = "Namespace",
                Value = "ns"
            }
        };



    public void OnGet() {
        StatusList = aggregation.GetHealthGroupBySrv();
    }


    [BindProperty]
    public string? GroupById { get; set; }

    public PartialViewResult OnPost() {
        if (GroupById is "ns")
            StatusList = aggregation.GetHealthGroupByNs();
        else if (GroupById is "srv")
            StatusList = aggregation.GetHealthGroupBySrv();

        return Partial("_SimpleList", StatusList);
    }
}
