import random, xkcd

if __name__ == "__main__":
    latest = xkcd.getLatestComicNum()
    for i in range(50):
        num = random.randint(1, latest)
        xkcd.Comic(num).download(output='.')
