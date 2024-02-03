using System;
using AutoMapper;
using Models;
using Models.Dto;
using Models.Dto.Book;
using Models.Dto.BookReading;
using Models.Dto.Comment;
using Models.Dto.Rating;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DataAccess.AutoMapper
{
	public class AutoMapperProfile : Profile
    {
		public AutoMapperProfile()
		{
            // book
            CreateMap<BookCreateDto, Book>();
            CreateMap<BookUpdateDto, Book>();
            CreateMap<BookDto, Book>();
            CreateMap<BooksDto, Book>();

            //
            CreateMap<Book, BookDto>();
            CreateMap<Book, BooksDto>();
            CreateMap<Book, BookCreateDto>();
            CreateMap<Book, BookUpdateDto>();

            // chapter
            CreateMap<ChapterCreateDto, Chapter>();
            CreateMap<ChapterUpdateDto, Chapter>();

            //
            CreateMap<Chapter, ChapterDto>();

            // author
            CreateMap<AuthorDto, Author>();
            CreateMap<AuthorCreateDto, Author>();
            CreateMap<AuthorUpdateDto, Author>();

            //
            CreateMap<Author, AuthorDto>();
            CreateMap<Author, AuthorCreateDto>();
            CreateMap<Author, AuthorUpdateDto>();

            // user
            CreateMap<ApplicationUser, ApplicationUserDto>();
            CreateMap<ApplicationUserDto, ApplicationUser>();

            // genre
            CreateMap<Genre, GenreDto>();

            //
            CreateMap<BookTotalPageResult, BookTotalPageResultDto>();

            // chinese book
            CreateMap<ChineseBook, ChineseBookDto>();

            CreateMap<ChineseBookDto, ChineseBook>();

            // bookreading
            CreateMap<BookReading, BookReadingDto>();
            CreateMap<BookReadingDto, BookReading>();

            // comment
            CreateMap<Comment, CommentCreateDto>();
            CreateMap<CommentCreateDto, Comment>();

            // rating
            CreateMap<Rating, RatingCreateDto>();
            CreateMap<Rating, RatingDto>();

            CreateMap<RatingDto, Rating>();
            CreateMap<RatingCreateDto, Rating>();
        }
	}
}

