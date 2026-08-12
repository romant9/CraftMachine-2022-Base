using System;

namespace BaseModel
{
	public class GroupModelBase
	{
		public string Id { get; set; }

		public long SequenceId { get; set; }

		public long LifeTime { get; set; }

		public DateTime Created { get; set; }

		public virtual float IndexScore => 0f;

		public virtual int Score { get; set; }

		public string Name { get; set; }

		public string Description { get; set; }

		public string NameLower
		{
			get
			{
				if (!string.IsNullOrEmpty(Name))
				{
					return Name.ToLowerInvariant();
				}
				return Name;
			}
		}

		public string DescriptionLower
		{
			get
			{
				if (!string.IsNullOrEmpty(Description))
				{
					return Description.ToLowerInvariant();
				}
				return Description;
			}
		}

		public string CountryCode { get; set; }

		public event GroupModelChangeEventHandler Changed;

		public GroupModelBase()
		{
		}

		public GroupModelBase(string id)
		{
			Id = id;
		}

		public void NotifyChange(string changed, object args = null)
		{
			if (this.Changed != null)
			{
				this.Changed(this, changed, args);
			}
		}

		public virtual void Tick(long deltaTime)
		{
			LifeTime += deltaTime;
		}
	}
}
