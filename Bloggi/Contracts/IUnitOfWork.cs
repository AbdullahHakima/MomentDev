using Bloggi.Models;
using Bloggi.Repositories;

namespace Bloggi.Contracts
{
    public interface IUnitOfWork:IDisposable
    {
       PostRepository Posts { get; }
       BaseRepository<Tag> Tags { get; }
       UserRepository Users { get; }
       BaseRepository<SocialLinks> SocialLinks { get; }

        void Complete();

    }
}
