using System;
using System.ComponentModel.DataAnnotations;

namespace Models.Dto
{
	public class AuthorDto
	{
        public int Id { get; set; }

        public string Name { get; set; }

        public AuthorDto()
        {

        }

        public AuthorDto(int id, string name)
        {
            this.Id = id;
            this.Name = name;
        }
    }
}

