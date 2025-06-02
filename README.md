# Reviews4everything backend API
## Descrption:
The backend api for www.reviews4everything.com. The api returns data to after fetching from a postgres database
## Optimization:
Benchmark obtained using k6 with 50 virtual concurrent users on my local machine. The tests are performed on the GET route used on the landing page of the frontend (api/reviews?page=page&limit=limit).
Where the page and limit variables are randomized on each request.
#### The baseline performence
Http request duration: avg=1730ms min=27.59ms med=1530ms max=9020ms    p(90)=3490ms p(95)=3540ms
#### With caching and optimized query  (91.6% faster than baseline)
Http request duration: avg=144.78ms min=0.5108ms med=2.36ms max=2670ms    p(90)=142.22ms p(95)=912.08ms


