using Dribbly.Core.Models;
using Dribbly.Model.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dribbly.Model.Tournaments
{
    public class TournamentViewerModel
    {
        public DateTime DateAdded { get; set; }
        public long Id { get; set; }
        public string Name { get; set; }
        public long AddedById { get; set; }
        public TournamentStatusEnum Status { get; set; }

        public TournamentViewerModel(TournamentModel model)
        {
            DateAdded = model.DateAdded;
            Id = model.Id;
            Name = model.Name;
            AddedById = model.AddedById;
            Status = model.Status;
        }
    }
}
