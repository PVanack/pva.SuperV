using Microsoft.AspNetCore.Components;

namespace pva.SuperV.Blazor.Client.Pages
{
    public class Timer : ComponentBase
    {
        [Parameter]
        public double TimeInSeconds { get; set; }
        [Parameter]
        public Action Tick { get; set; } = default!;
        protected override void OnInitialized()
        {
            var timer = new System.Threading.Timer(
                callback: (_) => InvokeAsync(() =>
                {
                    InvokeTick();
                }),
                state: null,
                dueTime: TimeSpan.FromSeconds(TimeInSeconds),
                period: TimeSpan.FromSeconds(TimeInSeconds));
        }

        private void InvokeTick()
        {
            Tick?.Invoke();
        }
    }
}
