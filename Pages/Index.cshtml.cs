using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gremlin.Net.Process.Traversal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace GremlinWeb.Pages
{
    public class IndexModel : PageModel
    {
        private readonly GraphTraversalSource _g;

        public IndexModel(GraphTraversalSource g)
        {
            _g = g;
        }

        public IList<IDictionary<string, object>> People { get; private set; }

        public async Task OnGetAsync()
        {
            People = await _g.V()
                .HasLabel("person")
                .Project<object>("ID", "FirstName", "LastName")
                    .By(T.Id)
                    .By("firstName")
                    .By("lastName")
                .Promise(traversal => traversal.ToList());
        }
    }
}
