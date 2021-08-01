﻿using AutoMapper;
using blastcms.web.Data;
using Marten;
using Marten.Linq;
using Marten.Pagination;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace blastcms.web.Handlers
{
    public class GetBlogArticles
    {
        public class Query : IRequest<Model>
        {
            public int Skip { get; internal set; }
            public int Take { get; internal set; }
            public int CurrentPage { get; internal set; }

            public Query(int skip, int take, int currentPage)
            {
                Skip = skip;
                Take = take;
                CurrentPage = currentPage;
            }
        }

        public class Model
        {
            public Model(IEnumerable<BlogArticle> articles, long count, int page)
            {
                Articles = articles;
                Count = count;
                Page = page;
            }

            public IEnumerable<BlogArticle> Articles { get;  }
            public long Count { get; }
            public int Page { get; }
        }


        public class AutoMapperProfile : Profile
        {
            public AutoMapperProfile()
            {

            }
        }

        public class Handler : IRequestHandler<Query, Model>
        {
            private readonly ISessionFactory _sessionFactory;
            private readonly IMapper _mapper;

            public Handler(ISessionFactory sessionFactory, IMapper mapper)
            {
                _sessionFactory = sessionFactory;
                _mapper = mapper;
            }

            public async Task<Model> Handle(Query request, CancellationToken cancellationToken)
            {
                using var session = _sessionFactory.QuerySession();
                {
                    QueryStatistics stats = null;

                    var articles = session.Query<BlogArticle>()
                        .Stats(out stats)
                        .Skip(request.Skip)
                        .Take(request.Take)
                        .OrderBy(o => o.Title);

                    return new Model(articles,stats.TotalResults, request.CurrentPage);
                }
            }

        }
    }
}
