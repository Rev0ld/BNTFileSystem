﻿using BussinessLayer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public class VideoContext : IDb<Video, string>
    {
        private readonly ApplicationDbContext dbContext;

        public VideoContext(ApplicationDbContext context)
        {
            dbContext = context;
        }

        public async Task CreateAsync(Video item)
        {
            try
            {
                Format? formatFromDb = await dbContext.Formats.FindAsync(item.Format.FormatId);

                if (formatFromDb != null)
                {
                    item.Format = formatFromDb;
                }

                dbContext.Videos.Add(item);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public async Task<Video> ReadAsync(string key, bool useNavigationalProperties = false, bool isReadOnly = true)
        {
            try
            {
                IQueryable<Video> query = dbContext.Videos;

                if (useNavigationalProperties)
                {
                    query = query.Include(v => v.Genres)
                        .Include(v => v.Authors)
                        .Include(v => v.Tags)
                        .Include(v => v.Format);
                }

                if (isReadOnly)
                {
                    query = query.AsNoTrackingWithIdentityResolution();
                }

                return await query.FirstOrDefaultAsync(v => v.VideoId == key);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public async Task<IEnumerable<Video>> ReadAllAsync(bool useNavigationalProperties = false, bool isReadOnly = true)
        {
            try
            {
                IQueryable<Video> query = dbContext.Videos;

                if (useNavigationalProperties)
                {
                    query = query.Include(v => v.Genres)
                        .Include(v => v.Authors)
                        .Include(v => v.Tags)
                        .Include(v => v.Format);
                }

                if (isReadOnly)
                {
                    query = query.AsNoTrackingWithIdentityResolution();
                }

                return await query.ToListAsync();
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public async Task UpdateAsync(Video item, bool useNavigationalProperties = false)
        {
            try
            {
                Video videoFromDb = await ReadAsync(item.VideoId, useNavigationalProperties, false);

                if (videoFromDb == null) { await CreateAsync(item); }

                dbContext.Entry<Video>(videoFromDb).CurrentValues.SetValues(item);

                if (useNavigationalProperties)
                {
                    Format? formatFromDb = await dbContext.Formats.FindAsync(item.Format.FormatId);

                    if (formatFromDb != null)
                    {
                        item.Format = formatFromDb;
                    }
                    else
                    {
                        item.Format = item.Format;
                    }

                    List<Author> authors = new List<Author>(item.Authors.Count);

                    foreach (var author in item.Authors)
                    {
                        Author? authorFromDb = await dbContext.Authors.FindAsync(author.AuthorId);

                        if (authorFromDb is null)
                        {
                            authors.Add(author);
                        }
                        else
                        {
                            authors.Add(authorFromDb);
                        }
                    }

                    List<Genre> genres = new List<Genre>(item.Genres.Count);

                    foreach (var genre in item.Genres)
                    {
                        Genre? genreFromDb = await dbContext.Genres.FindAsync(genre.GenreId);

                        if (genreFromDb is null)
                        {
                            genres.Add(genre);
                        }
                        else
                        {
                            genres.Add(genreFromDb);
                        }
                    }

                    List<Tag> tags = new List<Tag>(item.Tags.Count);

                    foreach (var tag in item.Tags)
                    {
                        Tag? tagFromDb = await dbContext.Tags.FindAsync(tag.TagId);

                        if (tagFromDb is null)
                        {
                            tags.Add(tag);
                        }
                        else
                        {
                            tags.Add(tagFromDb);
                        }
                    }

                    videoFromDb.Authors = authors;
                    videoFromDb.Genres = genres;
                    videoFromDb.Tags = tags;
                }

                await dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public async Task DeleteAsync(string key)
        {
            try
            {
                var videoFromDb = await ReadAsync(key, false, false);

                if(videoFromDb == null)
                {
                    throw new ArgumentException("The video with this Id does not exist");
                }

                dbContext.Videos.Remove(videoFromDb);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }
    }
}
