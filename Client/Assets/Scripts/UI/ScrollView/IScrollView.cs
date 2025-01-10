using System.Collections.Generic;

namespace GameEngine.UI
{
    public interface IScrollView<T>
    {
        void Initialize();
        void UpdateContent(IList<T> list);
        void SetVisible(bool visible);
    }
}