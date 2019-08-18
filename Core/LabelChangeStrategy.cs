using System.Threading.Tasks;

namespace Core
{
    public abstract class LabelChangeStrategy
    {
        public abstract Task ChangeLabelAsync();

        public class Add<TLabel, TApi> : LabelChangeStrategy where TLabel : ILabel where TApi : IApi<TLabel>
        {
            private readonly TApi api;
            private readonly TLabel newLabel;

            public Add(TApi api, TLabel newLabel)
            {
                this.api = api;
                this.newLabel = newLabel;
            }

            public override async Task ChangeLabelAsync()
            {
                await api.CreateLabelAsync(newLabel).ConfigureAwait(false);
            }

            public override string ToString()
            {
                return $"Add: {newLabel.ToString()}";
            }
        }

        public class Remove<TLabel, TApi> : LabelChangeStrategy where TLabel : ILabel where TApi : IApi<TLabel>
        {
            private readonly TApi api;
            private readonly TLabel oldLabel;

            public Remove(TApi api, TLabel oldLabel)
            {
                this.api = api;
                this.oldLabel = oldLabel;
            }

            public override async Task ChangeLabelAsync()
            {
                await api.DeleteLabelAsync(oldLabel).ConfigureAwait(false);
            }

            public override string ToString()
            {
                return $"Remoe: {oldLabel.ToString()}";
            }
        }

        public class Update<TLabel, TApi> : LabelChangeStrategy where TLabel : ILabel where TApi : IApi<TLabel>
        {
            private readonly TApi api;
            private readonly TLabel oldLabel;
            private readonly TLabel newLabel;

            public Update(TApi api, TLabel oldLabel, TLabel newLabel)
            {
                this.api = api;
                this.oldLabel = oldLabel;
                this.newLabel = newLabel;
            }

            public override async Task ChangeLabelAsync()
            {
                await api.UpdateLabelAsync(oldLabel, newLabel).ConfigureAwait(false);
            }

            public override string ToString()
            {
                return $"Update: {oldLabel.ToString()} ==> {newLabel.ToString()}";
            }
        }
    }
}
