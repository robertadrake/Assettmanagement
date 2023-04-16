using System.Collections.Generic;
using System.Threading.Tasks;
using Assettmanagement.Data;
using Assettmanagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Assettmanagement.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly DataAccess _dataAccess;
        public IndexModel(DataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }
        public List<Asset> Assets { get; set; }
        public async Task OnGetAsync()
        {
            Assets = await _dataAccess.GetAssetsWithUsersAsync();
        }
    }
}