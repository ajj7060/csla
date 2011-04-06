﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectTracker.Library;

namespace Mvc3UI.Controllers
{
  public class ProjectResourceController : Controller
  {
    //
    // GET: /ProjectResource/

    public ActionResult Index(int id)
    {
      var project = ProjectEdit.GetProject(id);
      ViewData.Add("ProjectId", project.Id);
      ViewData.Add("Title", project.Name);
      ViewData.Model = project.Resources;
      return View();
    }

    //
    // GET: /ProjectResource/Create

    public ActionResult Create(int projectId)
    {
      ViewData.Add("ProjectId", projectId);
      return View();
    }

    //
    // POST: /ProjectResource/Create

    [HttpPost]
    public ActionResult Create(int projectId, FormCollection collection)
    {
      try
      {
        var project = ProjectEdit.GetProject(projectId);
        var resourceId = int.Parse(collection["ResourceId"]);
        project.Resources.Assign(resourceId);
        var model = project.Resources.Where(r => r.ResourceId == resourceId).First();
        model.Role = int.Parse(collection["Role"]);
        project = project.Save();
        return RedirectToAction("Index", new { id = project.Id });
      }
      catch
      {
        ViewData.Add("ProjectId", projectId);
        ViewData.Model = new ProjectResourceEdit();
        return View();
      }
    }

    //
    // GET: /ProjectResource/Edit/5

    public ActionResult Edit(int projectId, int resourceId)
    {
      var project = ProjectEdit.GetProject(projectId);
      ViewData.Add("ProjectId", project.Id);
      ViewData.Model = project.Resources.Where(r => r.ResourceId == resourceId).First();
      return View();
    }

    //
    // POST: /ProjectResource/Edit/5

    [HttpPost]
    public ActionResult Edit(int projectId, int resourceId, FormCollection collection)
    {
      var project = ProjectEdit.GetProject(projectId);
      var model = project.Resources.Where(r => r.ResourceId == resourceId).First();
      try
      {
        model.Role = int.Parse(collection["Role"]);
        project = project.Save();
        return RedirectToAction("Index", new { id = project.Id });
      }
      catch
      {
        ViewData.Add("ProjectId", project.Id);
        ViewData.Model = project.Resources.Where(r => r.ResourceId == resourceId).First();
        return View();
      }
    }

    //
    // GET: /ProjectResource/Delete/5

    public ActionResult Delete(int projectId, int resourceId)
    {
      var project = ProjectEdit.GetProject(projectId);
      ViewData.Add("ProjectId", project.Id);
      ViewData.Model = project.Resources.Where(r => r.ResourceId == resourceId).First();
      return View();
    }

    //
    // POST: /ProjectResource/Delete/5

    [HttpPost]
    public ActionResult Delete(int projectId, int resourceId, FormCollection collection)
    {
      var project = ProjectEdit.GetProject(projectId);
      var model = project.Resources.Where(r => r.ResourceId == resourceId).First();
      try
      {
        project.Resources.Remove(model);
        project = project.Save();
        return RedirectToAction("Index", new { id = project.Id });
      }
      catch
      {
        ViewData.Add("ProjectId", project.Id);
        ViewData.Model = project.Resources.Where(r => r.ResourceId == resourceId).First();
        return View();
      }
    }
  }
}
