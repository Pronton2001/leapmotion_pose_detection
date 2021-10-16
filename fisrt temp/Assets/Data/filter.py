num_files = 6
output = open(f"features.csv", "w")
features_name=['thumb_x', 'thumb_y', 'thumb_z', 'index_x','index_y', 'index_z', 'middle_x', 'middle_y', 'middle_z', 'ring_x', 'ring_y', 'ring_z', 'pinky_x', 'pinky_y', 'pinky_z', 'palm_x', 'palm_y', 'palm_z']
heuristic = ['heuristic prediction']
target_name = ['ground truth']
msg0 = ','.join(features_name + heuristic+target_name + ['\n'])
# msg0 = features_name + heuristic + target_name + ['\n']
# print(str(msg0))
output.write(msg0)
for filename in range(num_files):
	with open(f'{filename}.txt', 'r') as f:
		line = f.readline()
		counter =0
		while line != '':  # The EOF char is an empty string
			if counter > 1000: break
			line = f.readline()
			if line == '': break # last column is empty
			msg = list(line)
			msg[-1] = ',' 
			msg += [str(filename),'\n']
			for i in range(len(msg)):
				if msg[i] =='(' or msg[i] == ')' or msg[i] == '': 
					continue
				output.write(msg[i])
			counter+=1