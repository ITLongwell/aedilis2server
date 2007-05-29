using System;
using System.Collections.Generic;
using Server;
using Server.Mobiles;
using Server.Gumps;
using Server.Items;
using Server.Multis;
using Fatima.Gumps;

namespace Fatima.Items
{
	public class ResourceBoxEntry
	{
		private Type m_Type;
		private string m_Category;
		private string m_ResName;
	
		public Type Type{ get{ return m_Type; } }
		public String Category{ get{ return m_Category; } }
		public String ResName{ get{ return m_ResName; } }
	
		public ResourceBoxEntry( Type type, string category, string resourceName)
		{
			m_Type = type;
			m_Category = category;
			m_ResName = resourceName;
		}
	}

	public class ResourceBoxContainer
	{
		private Dictionary<Type, int> m_Resources;

		public Dictionary<Type, int> Resources{ get{ return m_Resources; } }

		public ResourceBoxContainer()
		{
			m_Resources = new Dictionary<Type, int>();
		}

		public static Item CreateResource( Type t, int amount )
		{
			try
			{
				object o = Activator.CreateInstance( t );
				if (o != null && o is Item)
				{
					Item resource = (Item)o;
					resource.Amount = amount;

					return resource;
				}
			}
			catch{}

			return null;
		}

		public int GetAmount( Type resource )
		{
			if ( m_Resources.ContainsKey( resource ) )
				return m_Resources[resource];

			return 0;
		}

		public void SetAmount( Type resource, int amount )
		{
			if ( m_Resources.ContainsKey( resource ) )
				m_Resources[resource] = amount;
		}

		public void Add( Type resource, int amount )
		{
			if ( m_Resources.ContainsKey( resource ) )
				m_Resources[resource] += amount;
			else
				m_Resources.Add(resource, amount );
		}

		public void Serialize( GenericWriter writer )
		{
			lock( m_Resources )
			{
				writer.Write( (int)m_Resources.Keys.Count );
				foreach( Type t in m_Resources.Keys )
				{
					writer.Write( t.FullName ); //string
					writer.Write( m_Resources[t] ); //int
				}
			}
		}

		public static ResourceBoxContainer Deserialize( GenericReader reader )
		{
			ResourceBoxContainer box = new ResourceBoxContainer();

			int count = reader.ReadInt();
			for(int index=0;index<count;index++)
			{
				string resource = reader.ReadString();
				int amount = reader.ReadInt();

				Type entryType = Type.GetType( resource );

				if (entryType == null)
				{
					Console.WriteLine("\n\nWARNING!!! Resource Box detected an INVALID TYPE [{0}] saved on it. Please verify the box has references to the proper namespace. YOU MUST SHUTDOWN IMMEDIATELY TO PRESERVE DATA. World Save = Loss of Items", resource);
				}
				else
					box.Add( entryType, amount );
			}

			return box;
		}
	}

        [FlipableAttribute( 0xE41, 0xE40 )]
        public class ResourceBox : Item, ISecurable
        {
		private static Dictionary<Type, ResourceBoxEntry> m_ResourceEntries = new Dictionary<Type, ResourceBoxEntry>();
		private SecureLevel m_Level = SecureLevel.Anyone;

		public virtual Dictionary<Type, ResourceBoxEntry> ResourceEntries { get{ return m_ResourceEntries; } }
		public SecureLevel Level{ get{ return m_Level; } set{ m_Level = value; } }

		private ResourceBoxContainer m_Resources;
		const int RES_LIMIT_MAX = 100000;

                [Constructable]
                public ResourceBox() : base( 0xE41 )
                {
                        Movable = true;
                        Weight = 100.0;
                        Hue = 59;
                        Name = "Advanced Resource Box";

			m_Resources = new ResourceBoxContainer();
                }

		public void UpdateGump( Mobile to )
		{
			to.CloseGump( typeof( ResourceBoxGump ) );
			to.SendGump( new ResourceBoxGump( to, this, m_Resources ) );
		}

		public void UpdateGump( Mobile to, ResBoxGumpInfo info )
		{
			to.CloseGump( typeof( ResourceBoxGump ) );
			to.SendGump( new ResourceBoxGump( to, this, m_Resources, info.CatSelected, info.CatPage, info.ResPage ) );
		}

		public int GetAmount( Type resource )
		{
			return m_Resources.GetAmount(resource);
		}

		public void SetAmount( Type resource, int amount )
		{
			m_Resources.SetAmount( resource, amount );
		}

		public static string GetName( ResourceBox box, Type resource )
		{
			if (box.ResourceEntries.ContainsKey(resource))
				return box.ResourceEntries[resource].ResName;

			return "Unknown";
		}

		public static List<string> GetCategories( ResourceBox box )
		{
			List<string> cats = new List<string>();

			if (box.ResourceEntries.Keys.Count == 0)
				return cats;

			foreach( ResourceBoxEntry entry in box.ResourceEntries.Values )
			{
				if ( !cats.Contains( entry.Category ) )
					cats.Add( entry.Category );
			}

			return cats;
		}

		public static Dictionary<string, List<Type>> GetCategorizedTypes( ResourceBox box )
		{
			Dictionary<string, List<Type>> cats = new Dictionary<string, List<Type>>();

			if (box.ResourceEntries.Keys.Count == 0)
				return cats;

			foreach( ResourceBoxEntry entry in box.ResourceEntries.Values )
			{
				if ( !cats.ContainsKey( entry.Category ) )
					cats.Add( entry.Category, new List<Type>() );

				cats[entry.Category].Add( entry.Type );
			}

			return cats;
		}

		public static List<Type> GetTypes( ResourceBox box )
		{
			List<Type> types = new List<Type>();

			if (box.ResourceEntries.Keys.Count == 0)
				return types;

			foreach( ResourceBoxEntry entry in box.ResourceEntries.Values )
			{
				if ( !types.Contains( entry.Type ) )
					types.Add( entry.Type );
			}

			return types;
		}

		public static List<Type> GetTypes(  ResourceBox box, string category )
		{
			List<Type> types = new List<Type>();

			if (box.ResourceEntries.Keys.Count == 0)
				return types;

			foreach( ResourceBoxEntry entry in box.ResourceEntries.Values )
			{
				if ( !Insensitive.Equals(category,entry.Category) )
					types.Add( entry.Type );
			}

			return types;
		}

		static ResourceBox()
		{
			m_ResourceEntries.Add(typeof(IronIngot), new ResourceBoxEntry( typeof(IronIngot), "Ingots", "Iron Ingot" ));
			m_ResourceEntries.Add(typeof(DullCopperIngot), new ResourceBoxEntry( typeof(DullCopperIngot), "Ingots", "Dull Copper Ingot" ));
			m_ResourceEntries.Add(typeof(ShadowIronIngot), new ResourceBoxEntry( typeof(ShadowIronIngot), "Ingots", "Shadow Iron Ingot" ));
			m_ResourceEntries.Add(typeof(CopperIngot), new ResourceBoxEntry( typeof(CopperIngot), "Ingots", "Copper Ingot" ));
			m_ResourceEntries.Add(typeof(BronzeIngot), new ResourceBoxEntry( typeof(BronzeIngot), "Ingots", "Bronze Ingot" ));
			m_ResourceEntries.Add(typeof(GoldIngot), new ResourceBoxEntry( typeof(GoldIngot), "Ingots", "Golden Ingot" ));
			m_ResourceEntries.Add(typeof(AgapiteIngot), new ResourceBoxEntry( typeof(AgapiteIngot), "Ingots", "Agapite Ingot" ));
			m_ResourceEntries.Add(typeof(VeriteIngot), new ResourceBoxEntry( typeof(VeriteIngot), "Ingots", "Verite Ingot" ));
			m_ResourceEntries.Add(typeof(ValoriteIngot), new ResourceBoxEntry( typeof(ValoriteIngot), "Ingots", "Valorite Ingot" ));


			m_ResourceEntries.Add(typeof(Cloth), new ResourceBoxEntry( typeof(Cloth), "Tailoring", "Cloth" ));

			m_ResourceEntries.Add(typeof(JarHoney), new ResourceBoxEntry( typeof(JarHoney), "Baking", "Honey Jar" ));
		}

		public ResourceBox( Serial serial ) : base( serial )
		{
		}

		public override bool OnDragDrop( Mobile from, Item dropped )
		{
			if (dropped != null)
			{
				bool resAdd = AddResource(from, dropped,dropped.Amount);
				UpdateGump(from);
				return resAdd;
			}
			return false;
		}

		public override void GetContextMenuEntries( Mobile from, List<Server.ContextMenus.ContextMenuEntry> list )
		{
			base.GetContextMenuEntries( from, list );
			Server.Multis.SetSecureLevelEntry.AddTo( from, this, list );
		}

		public virtual bool CanAccess( Mobile from )
		{
			BaseHouse house = BaseHouse.FindHouseAt( this );

			if ( house != null )
			{
				return house.HasSecureAccess( from, m_Level ); //allows GMs to access always.
				/*
				switch ( m_Level )
				{
					case SecureLevel.Owner:
					{
						return house.IsOwner( from );
					}
					case SecureLevel.CoOwners:
					{
						return house.IsCoOwner( from );
					}
					case SecureLevel.Friends:
					{
						return house.IsFriend( from );
					}
					case SecureLevel.Anyone:
					{
						return true;
					}
					case SecureLevel.Guild:
					{
						return house.IsGuildMember( from );
					}
				}
				*/
			}
			else
				from.SendMessage("This is not located in someones house!");

			return false;
		}

                public override void OnDoubleClick( Mobile from )
                {
                        if (Movable)
                        {
                                from.SendMessage( "You must lock this item down in a household to use it." );
                                return;
                        }

                        if ( !from.InRange( GetWorldLocation(), 2 ) )
			{
                                from.LocalOverheadMessage( Server.Network.MessageType.Regular, 0x3B2, 1019045 ); // I can't reach that.
				return;
			}
                        else if ( from is PlayerMobile )
			{
				if ( CanAccess( from ) )
				{
                                	from.SendGump( new Fatima.Gumps.ResourceBoxGump( from, this, m_Resources ) );
				}
				else
					from.SendMessage("This box is locked; You cannot access it.");
			}
                }

		private void SendContentSpecs(Mobile from)
		{
			if ( m_Resources.Resources.Keys.Count == 0)
			{
				from.SendMessage("This resource box is empty.");
				return;
			}

			foreach( Type t in m_Resources.Resources.Keys )
			{
				if ( ResourceEntries.ContainsKey( t ) )
				{
					from.SendMessage( String.Format("{0} = {1}", ResourceEntries[t].ResName, m_Resources.Resources[t] ) );
				}
				else
				{
					from.SendMessage( String.Format("UNKNOWN TYPE [{0}] = {1}", t.FullName, m_Resources.Resources[t] ) );
				}

			}
		}

		private bool IsValidResource(Type type)
		{
			return ResourceEntries.ContainsKey(type);
		}

		public void Withdrawl( Mobile to, Type res, ResBoxGumpInfo info )
		{
			if (IsValidResource( res ) )
			{
				int amountAvail = m_Resources.GetAmount(res);

				if (amountAvail > 0)
				{
					to.SendMessage( String.Format("How much would you like to withdrawl? [{0} Max]", amountAvail) );
					to.Prompt = new WithdrawlPrompt(this, res, info);;

					
				}
				else
					to.SendMessage("You do not have any of that resource left!");
			}
		}

		private bool AddResource(Mobile from, Item item, int amount)
		{
			Type type = item.GetType();

			if (IsValidResource(type))
			{
				int storedAmount = m_Resources.GetAmount( type );
				if (amount + storedAmount > RES_LIMIT_MAX && RES_LIMIT_MAX - storedAmount > 0)
				{
					int resToFill = RES_LIMIT_MAX - storedAmount;
					m_Resources.Add( type, resToFill );

					int leftOvers = amount - resToFill;

					item.Amount = leftOvers;

					return false; //false, so it returns the stack.. though, updated.
				}
				else if ( storedAmount == RES_LIMIT_MAX )
				{
					from.SendMessage("That resource is completely full in this box! You cannot store anymore of it!");
					return false;
				}
				else
					m_Resources.Add( type, item.Amount );

				return true;
			}
			else
				from.SendMessage("That was not a valid resource to place inside the box.");

			return false;
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
		
			writer.WriteEncodedInt( (int) 0 ); // version

			if (m_Resources == null)
				m_Resources = new ResourceBoxContainer();

			m_Resources.Serialize( writer ); //Save the Contents of the box.
		}
		
		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
		
			int version = reader.ReadEncodedInt();
		
			m_Resources = ResourceBoxContainer.Deserialize(reader); //Load the Contents of the box
		}

		private class WithdrawlPrompt : Server.Prompts.Prompt
		{
			private ResourceBox m_Box;
			private Type m_Res;
			private ResBoxGumpInfo m_GumpInfo;

			public WithdrawlPrompt( ResourceBox box, Type res, ResBoxGumpInfo info )
			{
				m_Box = box;
				m_Res = res;
				m_GumpInfo = info;
			}

			public override void OnResponse( Mobile from, string text )
			{
				if ( m_Box == null )
					return;

				int amt = 0;
				try
				{
					amt = Int32.Parse(text);
				}
				catch
				{
					from.SendMessage("That was an invalid item amount. Your request has been canceled");
					return;
				}

				if ( !m_Box.CanAccess( from ) )
				{
					from.CloseGump( typeof( ResourceBoxGump ) );
					from.SendMessage("You cannot access this box. The lock has been changed!");
					return;
				}

				if ( amt >= 0 )
				{
					int stored = m_Box.GetAmount( m_Res );
					if (stored == 0)
					{
						from.SendMessage("Ooops! There is no longer any of that resource left!");
						return;
					}

					if ( stored - amt < 0 )
						amt = stored; //adjust it so that it only does as much as it can hold.

					Item resCreated = ResourceBoxContainer.CreateResource( m_Res, amt );
					if (resCreated != null)
					{
						m_Box.SetAmount( m_Res, (stored-amt) ); //set the remaining leftovers, if existant.
						resCreated.Amount = amt;

						from.AddToBackpack( resCreated );
						from.SendMessage( String.Format("You have pulled out {0} resources from the box", amt) );
					}

					m_Box.UpdateGump( from, m_GumpInfo );
				}
				else
					from.SendMessage("You'd love to add items, to the resource box.. wouldn't you?!");
			}
		}

	}
}