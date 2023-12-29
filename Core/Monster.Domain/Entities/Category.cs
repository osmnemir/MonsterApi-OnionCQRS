﻿using Monster.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monster.Domain.Entities
{
    public class Category:EntityBase
    {
        public  int ParentId { get; set; }
        public  string Name { get; set; }
        public  int Priorty { get; set; }
        public ICollection<Detail> Details { get; set; }
        public ICollection<Product> Products { get; set; }


        public Category()
        {

        }
        public Category(int parentId,string name,int priorty)
        {
            ParentId = parentId;
            Name = name;
            Priorty = priorty;
        }
    }
}
