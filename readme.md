-   Developed as a comprehensive tech forum web application using ASP.NET Core MVC 8, enabling users to ask and reply to questions with tags for searchable content.
-   Implemented a reward and penalty system based on accepted answers, upvotes, and downvotes to encourage quality participation.
-   Utilized Redis for rate limiting, Hangfire for background job processing, AWS SQS for email queuing, and AWS S3 for storing user attachments to enhance application performance and scalability.
-   Fully dockerized the application, including the SQL Server database, ensuring seamless deployment and efficient resource management.

---

Redis is added for:

-   caching member profile data which is repetitive in every request for showing data in nav bar.
-   Implementing email reset token rate limit. For each token issued, n \* 60 will be penaltied.
-   adding hangfire which is used for sending email parallelly. SQS Fifo queue is used along with it.

---

`docker compose up` needed to be run and .env file must be present at the root level for docker.
