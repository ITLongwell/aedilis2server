///////////////////////////////////////////////////
///// Created By Bad Karma aka Broadside///////////
///////////////////////////////////////////////////

using System;
using Server;

namespace Server.Items
{
	public class CyclopsCostume : Item, IDyable
	{

		public bool m_Transformed;
		public Timer m_TransformTimer;
		private DateTime m_End;

		[CommandProperty( AccessLevel.GameMaster )]
		public bool Transformed
		{
			get{ return m_Transformed; }
			set{ m_Transformed = value; }
		}

		[Constructable]
		public CyclopsCostume() : base( 0x1F03 )
		{
                        Name = "Cyclops Costume";
			
                        Hue = 2;
                        Layer = Layer.OuterTorso;
                        ItemID = 0x1F03;

			LootType = LootType.Blessed;
			Weight = 3.0;
		}

		public CyclopsCostume( Serial serial ) : base( serial )
		{
		}

     		public override void OnDoubleClick( Mobile from ) 
		{ 
			
                        if ( Parent != from ) 
                        { 
                                from.SendMessage( "The costume must be equiped to be used." ); 
                        } 

			else if ( from.Mounted == true )
			{
				from.SendMessage( "You cannot be mounted while wearing your costume!" );
			}


			else if ( from.BodyMod == 0x0 )
                        { 
				

               			from.SendMessage( "You pull the mask over your head." );
				from.PlaySound( 0x440 );
				from.BodyMod = 75;
				from.DisplayGuildTitle = false; 
                        
			}
			else
			{
				from.SendMessage( "You lower the mask." );
				from.PlaySound( 0x440 );
				from.BodyMod = 0x0;
				from.DisplayGuildTitle = true;
				this.Transformed = false;
			}
		}

		public virtual bool Dye( Mobile from, DyeTub sender )
		{
			if ( Deleted )
				return false;
			else if ( RootParent is Mobile && from != RootParent )
				return false;

			Hue = sender.DyedHue;

			return true;
		}
			
		public override void OnRemoved( Object parent)
      		{
      			
			            base.OnRemoved(parent);

            		if (parent is Mobile)
            		{
                		Mobile from = (Mobile)parent;

				if ( from.BodyMod == 101 )
                        	{ 
				
				from.SendMessage( "You lower the mask." );
				from.PlaySound( 0x440 );
				from.BodyMod = 0x0;
				from.DisplayGuildTitle = true;
				}
                        
			}
				
      			
      		}			

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
			ItemID = 0x1F03;
		}
	}
}
