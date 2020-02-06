#!/usr/bin/python
import os, sys
import csv

# Open a file
path = "/Users/jonastjomsland/Dissertation/data/screenshots"
dirs = os.listdir(path)

dict = {}

# # This would print all the files and directories
# for filename in dirs:
#     if filename[1] == "_":
#         dict[str(filename[0])] = filename
#     elif filename[2] == "_":
#         dict[str(filename[0:2])] = filename
#     elif filename[3] == "_":
#         dict[str(filename[0:3])] = filename
#     else:
#         dict[str(filename[0:4])] = filename

with open('data/features.csv','r') as csvinput:
    with open('images.csv', 'w') as csvoutput:
        writer = csv.writer(csvoutput, lineterminator='\n')
        reader = csv.reader(csvinput)
        all = []
        row = next(reader)
        row[43] = "image_url"
        all.append(row)
        i = 0
        for row in reader:
            row[43] = "https://acsdissertation.s3-eu-west-1.amazonaws.com/" + row[1][17:]
            all.append(row)
            i += 1
        writer.writerows(all)

# with open('images.csv', mode='w') as image_file:
#     image_file = csv.writer(image_file, delimiter=',', quotechar='"', quoting=csv.QUOTE_MINIMAL)
#     image_file.writerow(["image_url", "circle", "arrow"])
#     for i in range(5000):
#         if i < 10:
#             image_file.writerow(["https://acsfinalproject.s3.eu-west-2.amazonaws.com/" + dict[str(i)], dict[str(i)][2], dict[str(i)][4]])
#         if i > 9 and i < 100:
#             image_file.writerow(["https://acsfinalproject.s3.eu-west-2.amazonaws.com/" + dict[str(i)], dict[str(i)][3], dict[str(i)][5]])
#         if i > 99 and i < 1000:
#             image_file.writerow(["https://acsfinalproject.s3.eu-west-2.amazonaws.com/" + dict[str(i)], dict[str(i)][4], dict[str(i)][6]])
#         if i > 999:
#             image_file.writerow(["https://acsfinalproject.s3.eu-west-2.amazonaws.com/" + dict[str(i)], dict[str(i)][5], dict[str(i)][7]])
