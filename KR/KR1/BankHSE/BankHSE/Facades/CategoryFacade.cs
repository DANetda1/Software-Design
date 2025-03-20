using BankHSE.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BankHSE.Facades
{
    public class CategoryFacade
    {
        private readonly List<Category> _categories = new List<Category>();

        public Category CreateCategory(Category category)
        {
            _categories.Add(category);
            return category;
        }

        public void DeleteCategory(Guid categoryId)
        {
            var cat = _categories.FirstOrDefault(c => c.Id == categoryId);
            if (cat != null)
            {
                _categories.Remove(cat);
            }
        }

        public Category GetCategoryById(Guid categoryId)
        {
            return _categories.FirstOrDefault(c => c.Id == categoryId);
        }

        public IEnumerable<Category> GetAllCategories()
        {
            return _categories;
        }
    }
}