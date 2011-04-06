using Csla;
using System.ComponentModel.DataAnnotations;
using System;
using Csla.Serialization;
using System.ComponentModel;

namespace ProjectTracker.Library
{
  [Serializable]
  public class ProjectResourceEdit : BusinessBase<ProjectResourceEdit>
  {
    public static readonly PropertyInfo<byte[]> TimeStampProperty = RegisterProperty<byte[]>(c => c.TimeStamp);
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public byte[] TimeStamp
    {
      get { return GetProperty(TimeStampProperty); }
      set { SetProperty(TimeStampProperty, value); }
    }

    public static readonly PropertyInfo<int> ResourceIdProperty = 
      RegisterProperty<int>(c => c.ResourceId);
    [Display(Name = "Resource id")]
    public int ResourceId
    {
      get { return GetProperty(ResourceIdProperty); }
      private set { LoadProperty(ResourceIdProperty, value); }
    }

    public static readonly PropertyInfo<string> FirstNameProperty =
      RegisterProperty<string>(c => c.FirstName);
    [Display(Name = "First name")]
    public string FirstName
    {
      get { return GetProperty(FirstNameProperty); }
      private set { LoadProperty(FirstNameProperty, value); }
    }

    public static readonly PropertyInfo<string> LastNameProperty =
      RegisterProperty<string>(c => c.LastName);
    [Display(Name = "Last name")]
    public string LastName
    {
      get { return GetProperty(LastNameProperty); }
      private set { LoadProperty(LastNameProperty, value); }
    }

    public string FullName
    {
      get { return string.Format("{0}, {1}", LastName, FirstName); }
    }

    public static readonly PropertyInfo<DateTime> AssignedProperty =
      RegisterProperty<DateTime>(c => c.Assigned);
    [Display(Name = "Date assigned")]
    public DateTime Assigned
    {
      get { return GetProperty(AssignedProperty); }
      private set { LoadProperty(AssignedProperty, value); }
    }

    public static readonly PropertyInfo<int> RoleProperty = 
      RegisterProperty<int>(c => c.Role);
    [Display(Name = "Role assigned")]
    public int Role
    {
      get { return GetProperty(RoleProperty); }
      set { SetProperty(RoleProperty, value); }
    }

    public string RoleName
    {
      get 
      {
        var result = "none";
        if (RoleList.GetList().ContainsKey(Role))
          result = RoleList.GetList().GetItemByKey(Role).Value;
        return result;
      }
    }

    public override string ToString()
    {
      return ResourceId.ToString();
    }

    protected override void AddBusinessRules()
    {
      BusinessRules.AddRule(new Assignment.ValidRole(RoleProperty));

      BusinessRules.AddRule(new Csla.Rules.CommonRules.IsInRole(Csla.Rules.AuthorizationActions.WriteProperty, RoleProperty, "ProjectManager"));
    }

#if !SILVERLIGHT
    private void Child_Create(int resourceId)
    {
      using (BypassPropertyChecks)
      {
        ResourceId = resourceId;
        Role = RoleList.DefaultRole();
        LoadProperty(AssignedProperty, DateTime.Today);
        using (var ctx = ProjectTracker.Dal.DalFactory.GetManager())
        {
          var dal = ctx.GetProvider<ProjectTracker.Dal.IResourceDal>();
          var person = dal.Fetch(resourceId);
          FirstName = person.FirstName;
          LastName = person.LastName;
        }
      }
      base.Child_Create();
    }

    private void Child_Fetch(ProjectTracker.Dal.AssignmentDto data)
    {
      using (BypassPropertyChecks)
      {
        ResourceId = data.ResourceId;
        Role = data.RoleId;
        LoadProperty(AssignedProperty, data.Assigned);
        TimeStamp = data.LastChanged;
        using (var ctx = ProjectTracker.Dal.DalFactory.GetManager())
        {
          var dal = ctx.GetProvider<ProjectTracker.Dal.IResourceDal>();
          var person = dal.Fetch(data.ResourceId);
          FirstName = person.FirstName;
          LastName = person.LastName;
        }
      }
    }

    private void Child_Insert(ProjectEdit project)
    {
      using (var ctx = ProjectTracker.Dal.DalFactory.GetManager())
      {
        var dal = ctx.GetProvider<ProjectTracker.Dal.IAssignmentDal>();
        using (BypassPropertyChecks)
        {
          var item = new ProjectTracker.Dal.AssignmentDto
          {
            ProjectId = project.Id,
            ResourceId = this.ResourceId,
            Assigned = ReadProperty(AssignedProperty),
            RoleId = this.Role
          };
          dal.Insert(item);
          TimeStamp = item.LastChanged;
        }
      }
    }

    private void Child_Update(ProjectEdit project)
    {
      using (var ctx = ProjectTracker.Dal.DalFactory.GetManager())
      {
        var dal = ctx.GetProvider<ProjectTracker.Dal.IAssignmentDal>();
        using (BypassPropertyChecks)
        {
          var item = dal.Fetch(project.Id, ResourceId);
          item.Assigned = ReadProperty(AssignedProperty);
          item.RoleId = Role;
          dal.Update(item);
          TimeStamp = item.LastChanged;
        }
      }
    }

    private void Child_DeleteSelf(ProjectEdit project)
    {
      using (var ctx = ProjectTracker.Dal.DalFactory.GetManager())
      {
        var dal = ctx.GetProvider<ProjectTracker.Dal.IAssignmentDal>();
        using (BypassPropertyChecks)
        {
          dal.Delete(project.Id, ResourceId);
        }
      }
    }
#endif
  }
}