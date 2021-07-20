﻿using AutoMapper;
using blastcms.web.Data;
using Marten;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace blastcms.web.Handlers
{
    public class AlterBlogArticle
    {
        public class Command : IRequest<Model>
        {
            public long? Id { get; set; }
            public string Title { get; set; }
            public string Author { get; set; }
            public DateTime PublishedDate { get; set; }
            public string ImageUrl { get; set; }
            public IEnumerable<string> BlogTags { get; set; }
            public string Description { get; set; }
            public string Body { get; set; }
            public string Slug { get; set; }

        }

        public class Model
        {
            public Model(BlogArticle article)
            {
                Article = article;
            }

            public BlogArticle Article { get; }
        }


        public class AutoMapperProfile : Profile
        {
            public AutoMapperProfile()
            {
                CreateMap<Command, BlogArticle>().ReverseMap();
            }
        }

        public class Handler : IRequestHandler<Command, Model>
        {
            private readonly ISessionFactory _sessionFactory;
            private readonly IMapper _mapper;

            public Handler(ISessionFactory sessionFactory, IMapper mapper)
            {
                _sessionFactory = sessionFactory;
                _mapper = mapper;
            }

            public async Task<Model> Handle(Command request, CancellationToken cancellationToken)
            {
                var article = _mapper.Map<BlogArticle>(request);

                using var session = _sessionFactory.OpenSession();
                {
                    session.Store(article);

                    session.SaveChanges();

                    return new Model(article);
                }
            }

        }
    }
}

