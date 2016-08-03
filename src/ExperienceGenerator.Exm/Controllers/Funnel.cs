﻿namespace ExperienceGenerator.Exm.Controllers
{
  public class Funnel
  {
    public int Bounced { get; set; }
    public int ClickRate { get; set; }
    public int Delivered { get; set; }
    public int OpenRate { get; set; }
    public int SpamComplaints { get; set; }
#warning not used
    public int TotalSent { get; set; }
    public int UniqueClickRate { get; set; }
    public int UniqueOpenRate { get; set; }
    public int Unsubscribed { get; set; }

  }
}