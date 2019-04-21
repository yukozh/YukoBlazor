using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using YukoBlazor.Shared;

namespace YukoBlazor.ApiInvoker
{
    public partial class ApiClient
    {
        public async Task<IEnumerable<CalendarViewModel>> GetCalendarAsync(
            CancellationToken token = default)
        {
            return await client.GetJsonAsync<IEnumerable<CalendarViewModel>>("/api/Calendar");
        }
    }
}
