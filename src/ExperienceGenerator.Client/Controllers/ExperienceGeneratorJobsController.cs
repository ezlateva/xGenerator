﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Sitecore;

namespace ExperienceGenerator.Client.Controllers
{
    public class ExperienceGeneratorJobsController : ApiController
    {        
        public IHttpActionResult Post([FromBody] JobSpecification spec)
        {
            if (string.IsNullOrEmpty(spec.RootUrl))
            {
                spec.RootUrl = Request.RequestUri.GetLeftPart(UriPartial.Authority);
            }
            if (spec.Specification == null)
            {
                throw new Exception("Specification expected");
            }

            var status = XGenJobManager.Instance.StartNew(spec);
            return RedirectToRoute("ExperienceGeneratorJobsApi", new { id = status.Id });
        }

        public IEnumerable<JobInfo> Get()
        {
            return XGenJobManager.Instance.Jobs.Where(job => job.JobStatus <= JobStatus.Cancelling)
                .Select(UpdateStatusUrl);
        }    

        public IHttpActionResult Get(Guid id)
        {
            //TODO: Work around for blocked PUT issue
            var pause = Context.Request.QueryString["pause"];
            if (!string.IsNullOrEmpty(pause))
            {
                return Put(id, pause == "true");
            }

            var job = XGenJobManager.Instance.Poll(id);

            if (job != null)
           {                
                return Ok(UpdateStatusUrl(job));
            }

            return NotFound();
        }

        //TODO: GRRR! Something is blocking PUT requests in Sitecore. Probably WebDAV.
        [HttpPut]
        public IHttpActionResult Put(Guid id, bool pause)
        {
            var job = XGenJobManager.Instance.Poll(id);

            if (job != null)
            {
                if (pause)
                {
                    job.Pause();
                }
                else
                {
                    job.Resume();
                }
                return Ok(UpdateStatusUrl(job));
            }

            return NotFound();
        }
        
        public IHttpActionResult Delete(Guid id)
        {
            var job = XGenJobManager.Instance.Poll(id);

            if (job != null)
            {
                job.Stop();
                return Ok(UpdateStatusUrl(job));
            }

            return NotFound();
        }


        JobInfo UpdateStatusUrl(JobInfo job)
        {
            job.StatusUrl = Url.Route("ExperienceGeneratorJobsApi", new { action = "get", id = job.Id });
            return job;
        }
    }
}