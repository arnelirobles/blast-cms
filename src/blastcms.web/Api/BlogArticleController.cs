﻿using blastcms.web.Attributes;
using blastcms.web.Data;
using blastcms.web.Handlers;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace blastcms.web.Api
{
    [Route("api")]
    [ApiController]
    [ApiVersion("1.0")]
    [ApiKey]
    public class BlogArticleController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BlogArticleController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("blogarticle/all")]
        public async Task<ActionResult<IEnumerable<BlogArticle>>> GetAll([FromQuery] int skip = 0, [FromQuery] int take = 10, [FromQuery] int currentPage=0, [FromQuery] string search=null)
        {
            var results = await _mediator.Send(new GetBlogArticles.Query(skip, take, currentPage, search));

            return results.Articles.ToArray();
        }

        [HttpGet("blogarticle/{id}")]
        public async Task<ActionResult<BlogArticle>> Get(Guid id)
        {
            var results = await _mediator.Send(new GetBlogArticle.Query(id));

            return results.Article;
        }

    }
}
