using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mymusic_app.Migrations
{
    /// <inheritdoc />
    public partial class mig4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlaylistFollows_Playlists_PlaylistId",
                table: "PlaylistFollows");

            migrationBuilder.DropForeignKey(
                name: "FK_PlaylistSongs_Songs_SongId",
                table: "PlaylistSongs");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAlbumFollows_Albums_AlbumId",
                table: "UserAlbumFollows");

            migrationBuilder.DropForeignKey(
                name: "FK_UserArtistFollows_Artists_ArtistId",
                table: "UserArtistFollows");

            migrationBuilder.DropForeignKey(
                name: "FK_UserSongLikes_Songs_SongId",
                table: "UserSongLikes");

            migrationBuilder.DropForeignKey(
                name: "FK_UserSongPlays_Songs_SongId",
                table: "UserSongPlays");

            migrationBuilder.AddForeignKey(
                name: "FK_PlaylistFollows_Playlists_PlaylistId",
                table: "PlaylistFollows",
                column: "PlaylistId",
                principalTable: "Playlists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlaylistSongs_Songs_SongId",
                table: "PlaylistSongs",
                column: "SongId",
                principalTable: "Songs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserAlbumFollows_Albums_AlbumId",
                table: "UserAlbumFollows",
                column: "AlbumId",
                principalTable: "Albums",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserArtistFollows_Artists_ArtistId",
                table: "UserArtistFollows",
                column: "ArtistId",
                principalTable: "Artists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserSongLikes_Songs_SongId",
                table: "UserSongLikes",
                column: "SongId",
                principalTable: "Songs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserSongPlays_Songs_SongId",
                table: "UserSongPlays",
                column: "SongId",
                principalTable: "Songs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlaylistFollows_Playlists_PlaylistId",
                table: "PlaylistFollows");

            migrationBuilder.DropForeignKey(
                name: "FK_PlaylistSongs_Songs_SongId",
                table: "PlaylistSongs");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAlbumFollows_Albums_AlbumId",
                table: "UserAlbumFollows");

            migrationBuilder.DropForeignKey(
                name: "FK_UserArtistFollows_Artists_ArtistId",
                table: "UserArtistFollows");

            migrationBuilder.DropForeignKey(
                name: "FK_UserSongLikes_Songs_SongId",
                table: "UserSongLikes");

            migrationBuilder.DropForeignKey(
                name: "FK_UserSongPlays_Songs_SongId",
                table: "UserSongPlays");

            migrationBuilder.AddForeignKey(
                name: "FK_PlaylistFollows_Playlists_PlaylistId",
                table: "PlaylistFollows",
                column: "PlaylistId",
                principalTable: "Playlists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PlaylistSongs_Songs_SongId",
                table: "PlaylistSongs",
                column: "SongId",
                principalTable: "Songs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserAlbumFollows_Albums_AlbumId",
                table: "UserAlbumFollows",
                column: "AlbumId",
                principalTable: "Albums",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserArtistFollows_Artists_ArtistId",
                table: "UserArtistFollows",
                column: "ArtistId",
                principalTable: "Artists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserSongLikes_Songs_SongId",
                table: "UserSongLikes",
                column: "SongId",
                principalTable: "Songs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserSongPlays_Songs_SongId",
                table: "UserSongPlays",
                column: "SongId",
                principalTable: "Songs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
