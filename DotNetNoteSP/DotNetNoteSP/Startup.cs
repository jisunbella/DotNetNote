using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNetNote.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DotNetNote
{
    public class Startup
    { 
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddSingleton<IConfiguration>(Configuration);
            // 각각의 repository 클래스의 생성자에서 Configuration 개체를 통해서 appsettings.json 파일에 등록된 데이터베이스 연결 문자열을 사용할 수 있도록 설정하는 코드

            services.AddTransient<IBoardRepository, BoardRepository>(); // 기본 방식
            //게시판 관련 서비스 등록, DotNetNote Controller에서 생성자 주입 방식으로 INoteRepository를 넘겨주면 컨트롤러 실행 시 자동으로 NoteRepository 클래스의 인스턴스를 생성해주는 역할

            //services.AddSingleton<IBoardRepository>(new BoardRepository(Configuration["ConnectionStrings:DefaultConnection"]));
            //게시판 댓글 관련 서비스 등록, 싱글톤 메서드를 이용해 Startup.cs파일에서 NoteCommentaryRepository 클래스 생성자에 데이터베이스 연결 문자열을 전송하는 방식을 사용

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Board}/{action=Index}/{id?}");
            });
        }
    }
}
