using System.Threading.Tasks;

namespace VarelaAloisio.Core
{
    public interface IView
    {
        /// <summary /> Shows the object
        Task Show();
        /// <summary /> Hides the object
        Task Hide();
    }
}