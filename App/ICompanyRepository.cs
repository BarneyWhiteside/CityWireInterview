using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace App
{
    public interface ICompanyRepository
    {
        Company GetById(int id);
    }
}
