using Bloggi.Contracts;
using Bloggi.Data;
using Bloggi.Models;
using Microsoft.EntityFrameworkCore;

namespace Bloggi.Repositories
{
    public class UnitOfWork:IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        public PostRepository Posts { get; private set; }
        public Contracts.BaseRepository<Tag> Tags { get; private set; }
        public UserRepository Users { get; private set; }

        public Contracts.BaseRepository<SocialLinks> SocialLinks { get; private set; }

		public Contracts.BaseRepository<Subscriber> Subscribers  {get; private set; }


		public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Posts = new PostRepository(_context);
            Tags = new BaseRepository<Tag>(_context);
            Users = new UserRepository(_context);
            SocialLinks = new BaseRepository<SocialLinks>(_context);
            Subscribers=new BaseRepository<Subscriber>(_context); 
            
        }
        public void Complete()
        {
            _context.SaveChanges();
        }
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
